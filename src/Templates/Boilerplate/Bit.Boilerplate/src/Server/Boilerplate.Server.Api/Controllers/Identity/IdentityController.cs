﻿//+:cnd:noEmit
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Boilerplate.Server.Api.Services;
using Boilerplate.Shared.Dtos.Identity;
using Boilerplate.Server.Api.Models.Identity;
using Boilerplate.Shared.Controllers.Identity;

namespace Boilerplate.Server.Api.Controllers.Identity;

[ApiController, AllowAnonymous]
[Route("api/[controller]/[action]")]
public partial class IdentityController : AppControllerBase, IIdentityController
{
    [AutoInject] private SmsService smsService = default!;
    [AutoInject] private EmailService emailService = default!;
    [AutoInject] private HtmlRenderer htmlRenderer = default!;
    [AutoInject] private IUserStore<User> userStore = default!;
    [AutoInject] private UserManager<User> userManager = default!;
    [AutoInject] private SignInManager<User> signInManager = default!;
    [AutoInject] private ILogger<IdentityController> logger = default!;
    [AutoInject] private IUserConfirmation<User> userConfirmation = default!;
    [AutoInject] private IOptionsMonitor<BearerTokenOptions> bearerTokenOptions = default!;

    //#if (captcha == "reCaptcha")
    [AutoInject] private GoogleRecaptchaHttpClient googleRecaptchaHttpClient = default!;
    //#endif

    /// <summary>
    /// By leveraging summary tags in your controller's actions and DTO properties you can make your codes much easier to maintain.
    /// These comments will also be used in swagger docs and ui.
    /// </summary>
    [HttpPost]
    public async Task SignUp(SignUpRequestDto request, CancellationToken cancellationToken)
    {
        //#if (captcha == "reCaptcha")
        if (await googleRecaptchaHttpClient.Verify(request.GoogleRecaptchaResponse, cancellationToken) is false)
            throw new BadRequestException(Localizer[nameof(AppStrings.InvalidGoogleRecaptchaResponse)]);
        //#endif

        // Attempt to locate an existing user using either their email address or phone number. The enforcement of a unique username policy is integral to the aspnetcore identity framework.
        var existingUser = await userManager.FindUserAsync(new() { Email = request.Email, PhoneNumber = request.PhoneNumber });
        if (existingUser is not null)
            throw new BadRequestException(Localizer[nameof(AppStrings.DuplicateEmailOrPhoneNumber)]);

        var userToAdd = new User { LockoutEnabled = true };

        await userStore.SetUserNameAsync(userToAdd, request.UserName!, cancellationToken);

        if (string.IsNullOrEmpty(request.Email) is false)
        {
            await ((IUserEmailStore<User>)userStore).SetEmailAsync(userToAdd, request.Email!, cancellationToken);
        }

        if (string.IsNullOrEmpty(request.PhoneNumber) is false)
        {
            await ((IUserPhoneNumberStore<User>)userStore).SetPhoneNumberAsync(userToAdd, request.PhoneNumber!, cancellationToken);
        }

        var result = await userManager.CreateAsync(userToAdd, request.Password!);

        if (result.Succeeded is false)
        {
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());
        }

        if (string.IsNullOrEmpty(userToAdd.Email) is false)
        {
            await SendConfirmEmailToken(userToAdd, cancellationToken);
        }

        if (string.IsNullOrEmpty(userToAdd.PhoneNumber) is false)
        {
            await SendConfirmPhoneToken(userToAdd, cancellationToken);
        }
    }

    [HttpPost]
    public async Task SendConfirmEmailToken(SendEmailTokenRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email!)
            ?? throw new BadRequestException(Localizer[nameof(AppStrings.UserNotFound)]);

        if (await userManager.IsEmailConfirmedAsync(user))
            throw new BadRequestException(Localizer[nameof(AppStrings.EmailAlreadyConfirmed)]);

        await SendConfirmEmailToken(user, cancellationToken);
    }

    [HttpPost]
    public async Task ConfirmEmail(ConfirmEmailRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email!)
            ?? throw new BadRequestException(Localizer[nameof(AppStrings.UserNotFound)]);

        if (await userManager.IsEmailConfirmedAsync(user)) return;

        if (await userManager.IsLockedOutAsync(user))
            throw new BadRequestException(Localizer[nameof(AppStrings.UserLockedOut), (DateTimeOffset.UtcNow - user.LockoutEnd!).Value.ToString("mm\\:ss")]);

        var tokenIsValid = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, FormattableString.Invariant($"VerifyEmail:{request.Email},{user.EmailTokenRequestedOn}"), request.Token!);

        if (tokenIsValid is false)
        {
            await userManager.AccessFailedAsync(user);
            throw new BadRequestException();
        }

        var userEmailStore = (IUserEmailStore<User>)userStore;
        await userEmailStore.SetEmailConfirmedAsync(user, true, cancellationToken);
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded is false)
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());

        await ((IUserLockoutStore<User>)userStore).ResetAccessFailedCountAsync(user, cancellationToken);
        user.EmailTokenRequestedOn = null; // invalidates email token
        var updateResult = await userManager.UpdateAsync(user);
        if (updateResult.Succeeded is false)
            throw new ResourceValidationException(updateResult.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());
    }

    [HttpPost]
    public async Task SendConfirmPhoneToken(SendPhoneTokenRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByPhoneNumber(request.PhoneNumber!)
            ?? throw new BadRequestException(Localizer[nameof(AppStrings.UserNotFound)]);

        if (await userManager.IsPhoneNumberConfirmedAsync(user))
            throw new BadRequestException(Localizer[nameof(AppStrings.PhoneNumberAlreadyConfirmed)]);

        await SendConfirmPhoneToken(user, cancellationToken);
    }

    [HttpPost]
    public async Task ConfirmPhone(ConfirmPhoneRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByPhoneNumber(request.PhoneNumber!)
            ?? throw new BadRequestException(Localizer[nameof(AppStrings.UserNotFound)]);

        if (await userManager.IsLockedOutAsync(user))
            throw new BadRequestException(Localizer[nameof(AppStrings.UserLockedOut), (DateTimeOffset.UtcNow - user.LockoutEnd!).Value.ToString("mm\\:ss")]);

        if (await userManager.IsPhoneNumberConfirmedAsync(user)) return;

        var tokenIsValid = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, FormattableString.Invariant($"VerifyPhoneNumber:{request.PhoneNumber},{user.PhoneNumberTokenRequestedOn}"), request.Token!);

        if (tokenIsValid is false)
        {
            await userManager.AccessFailedAsync(user);
            throw new BadRequestException();
        }
        await ((IUserPhoneNumberStore<User>)userStore).SetPhoneNumberConfirmedAsync(user, true, cancellationToken);
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded is false)
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());

        await ((IUserLockoutStore<User>)userStore).ResetAccessFailedCountAsync(user, cancellationToken);
        user.PhoneNumberTokenRequestedOn = null; // invalidates phone token
        var updateResult = await userManager.UpdateAsync(user);
        if (updateResult.Succeeded is false)
            throw new ResourceValidationException(updateResult.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());
    }

    [HttpPost]
    public async Task<ActionResult<SignInResponseDto>> SignIn(SignInRequestDto request, CancellationToken cancellationToken)
    {
        signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

        var user = await userManager.FindUserAsync(request) ?? throw new UnauthorizedException(Localizer[nameof(AppStrings.InvalidUserCredentials)]);

        var result = string.IsNullOrEmpty(request.Otp) is false
            ? await signInManager.OtpSignInAsync(user, request.Otp!)
            : await signInManager.PasswordSignInAsync(user!.UserName!, request.Password!, isPersistent: false, lockoutOnFailure: true);

        if (result.IsNotAllowed && await userConfirmation.IsConfirmedAsync(userManager, user) is false)
            throw new BadRequestException(Localizer[nameof(AppStrings.UserIsNotConfirmed)]);

        if (result.IsLockedOut)
            throw new BadRequestException(Localizer[nameof(AppStrings.UserLockedOut), (DateTimeOffset.UtcNow - user.LockoutEnd!).Value.ToString("mm\\:ss")]);

        if (result.RequiresTwoFactor)
        {
            if (string.IsNullOrEmpty(request.TwoFactorRecoveryCode) is false)
            {
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(request.TwoFactorRecoveryCode);
            }
            else if (string.IsNullOrEmpty(request.TwoFactorToken) is false)
            {
                result = await signInManager.TwoFactorSignInAsync(TokenOptions.DefaultPhoneProvider, request.TwoFactorToken, false, false);
            }
            else if (string.IsNullOrEmpty(request.TwoFactorCode) is false)
            {
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(request.TwoFactorCode, false, false);
            }
            else
            {
                return new SignInResponseDto { RequiresTwoFactor = true };
            }
        }

        if (result.Succeeded is false)
            throw new UnauthorizedException(Localizer[nameof(AppStrings.InvalidUserCredentials)]);

        if (string.IsNullOrEmpty(request.Otp) is false)
        {
            await ((IUserLockoutStore<User>)userStore).ResetAccessFailedCountAsync(user, cancellationToken);
            user.OtpRequestedOn = null; // invalidates the OTP
            var updateResult = await userManager.UpdateAsync(user);
            if (updateResult.Succeeded is false)
                throw new ResourceValidationException(updateResult.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());
        }

        return Empty;
    }

    [HttpPost]
    public async Task<ActionResult<TokenResponseDto>> Refresh(RefreshRequestDto request)
    {
        var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(request.RefreshToken);

        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc || DateTimeOffset.UtcNow >= expiresUtc ||
                await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)
        {
            // Return 401 if refresh token is either invalid or expired.
            throw new UnauthorizedException();
        }

        var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);

        return SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }

    [HttpPost]
    public async Task SendResetPasswordToken(SendResetPasswordTokenRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindUserAsync(request)
                    ?? throw new ResourceNotFoundException(Localizer[nameof(AppStrings.UserNotFound)]);

        if (await userConfirmation.IsConfirmedAsync(userManager, user) is false)
            throw new BadRequestException(Localizer[nameof(AppStrings.UserIsNotConfirmed)]);

        var resendDelay = (DateTimeOffset.Now - user.ResetPasswordTokenRequestedOn) - AppSettings.Identity.ResetPasswordTokenRequestResendDelay;

        if (resendDelay < TimeSpan.Zero)
            throw new TooManyRequestsExceptions(Localizer[nameof(AppStrings.WaitForResetPasswordTokenRequestResendDelay), resendDelay.Value.ToString("mm\\:ss")]);

        user.ResetPasswordTokenRequestedOn = DateTimeOffset.Now;

        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded is false)
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());

        var token = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, FormattableString.Invariant($"ResetPassword,{user.ResetPasswordTokenRequestedOn}"));
        var isEmail = string.IsNullOrEmpty(request.Email) is false;
        var qs = $"{(isEmail ? "email" : "phoneNumber")}={Uri.EscapeDataString(isEmail ? request.Email! : request.PhoneNumber!)}";
        var url = $"reset-password?token={Uri.EscapeDataString(token)}&{qs}&culture={CultureInfo.CurrentUICulture.Name}";
        var link = new Uri(HttpContext.Request.GetWebClientUrl(), url);

        async Task SendEmail()
        {
            if (await userManager.IsEmailConfirmedAsync(user) is false) return;

            await emailService.SendResetPasswordToken(user, token, link, cancellationToken);
        }

        async Task SendSms()
        {
            if (await userManager.IsPhoneNumberConfirmedAsync(user) is false) return;

            await smsService.SendSms(Localizer[nameof(AppStrings.ResetPasswordTokenSmsText), token], user.PhoneNumber!, cancellationToken);
        }

        await Task.WhenAll([SendEmail(), SendSms()]);
    }

    /// <summary>
    /// For either otp or magic link
    /// </summary>
    [HttpPost]
    public async Task SendOtp(IdentityRequestDto request, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindUserAsync(request)
                    ?? throw new ResourceNotFoundException(Localizer[nameof(AppStrings.UserNotFound)]);

        if (await userConfirmation.IsConfirmedAsync(userManager, user) is false)
            throw new BadRequestException(Localizer[nameof(AppStrings.UserIsNotConfirmed)]);

        var resendDelay = (DateTimeOffset.Now - user.OtpRequestedOn) - AppSettings.Identity.OtpRequestResendDelay;

        if (resendDelay < TimeSpan.Zero)
            throw new TooManyRequestsExceptions(Localizer[nameof(AppStrings.WaitForOtpRequestResendDelay), resendDelay.Value.ToString("mm\\:ss")]);

        var (token, url) = await GenerateOtpTokenData(user, returnUrl);

        var link = new Uri(HttpContext.Request.GetWebClientUrl(), url);

        async Task SendEmail()
        {
            if (await userManager.IsEmailConfirmedAsync(user) is false) return;

            await emailService.SendOtp(user, token, link, cancellationToken);
        }

        async Task SendSms()
        {
            if (await userManager.IsPhoneNumberConfirmedAsync(user) is false) return;

            await smsService.SendSms(Localizer[nameof(AppStrings.OtpSmsText), token], user.PhoneNumber!, cancellationToken);
        }

        await Task.WhenAll([SendEmail(), SendSms()]);
    }

    [HttpPost]
    public async Task ResetPassword(ResetPasswordRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindUserAsync(request) ?? throw new ResourceNotFoundException(Localizer[nameof(AppStrings.UserNotFound)]);

        if (await userManager.IsLockedOutAsync(user))
            throw new BadRequestException(Localizer[nameof(AppStrings.UserLockedOut), (DateTimeOffset.UtcNow - user.LockoutEnd!).Value.ToString("mm\\:ss")]);

        bool tokenIsValid = await userManager.VerifyUserTokenAsync(user!, TokenOptions.DefaultPhoneProvider, FormattableString.Invariant($"ResetPassword,{user.ResetPasswordTokenRequestedOn}"), request.Token!);

        if (tokenIsValid is false)
        {
            await userManager.AccessFailedAsync(user);
            throw new BadRequestException();
        }

        var result = await userManager.ResetPasswordAsync(user!, await userManager.GeneratePasswordResetTokenAsync(user!), request.Password!);

        if (result.Succeeded is false)
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());

        await ((IUserLockoutStore<User>)userStore).ResetAccessFailedCountAsync(user, cancellationToken);
        user.ResetPasswordTokenRequestedOn = null; // invalidates reset password token
        var updateResult = await userManager.UpdateAsync(user);
        if (updateResult.Succeeded is false)
            throw new ResourceValidationException(updateResult.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());
    }

    [HttpPost]
    public async Task SendTwoFactorToken(IdentityRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindUserAsync(request) ?? throw new ResourceNotFoundException(Localizer[nameof(AppStrings.UserNotFound)]);

        var resendDelay = (DateTimeOffset.Now - user.TwoFactorTokenRequestedOn) - AppSettings.Identity.TwoFactorTokenRequestResendDelay;

        if (resendDelay < TimeSpan.Zero)
            throw new TooManyRequestsExceptions(Localizer[nameof(AppStrings.WaitForTwoFactorTokenRequestResendDelay), resendDelay.Value.ToString("mm\\:ss")]);

        user.TwoFactorTokenRequestedOn = DateTimeOffset.Now;
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded is false)
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());

        var token = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

        async Task SendEmail()
        {
            if (await userManager.IsEmailConfirmedAsync(user))
            {
                await emailService.SendTwoFactorToken(user, token, cancellationToken);
            }
        }

        async Task SendSms()
        {
            if (await userManager.IsPhoneNumberConfirmedAsync(user))
            {
                await smsService.SendSms(Localizer[nameof(AppStrings.TwoFactorTokenSmsText), token], user.PhoneNumber!, cancellationToken);
            }
        }

        await Task.WhenAll([SendEmail(), SendSms()]);
    }

    [HttpGet]
    public async Task<string> GetSocialSignInUri(string provider, string? returnUrl = null, int? localHttpPort = null)
    {
        var uri = Url.Action(nameof(SocialSignIn), new { provider, returnUrl, localHttpPort })!;
        return new Uri(Request.GetBaseUrl(), uri).ToString();
    }

    [HttpGet]
    public async Task<ActionResult> SocialSignIn(string provider, string? returnUrl = null, int? localHttpPort = null)
    {
        var redirectUrl = Url.Action(nameof(SocialSignInCallback), "Identity", new { returnUrl, localHttpPort });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    public async Task<ActionResult> SocialSignInCallback(string? returnUrl = null, int? localHttpPort = null, CancellationToken cancellationToken = default)
    {
        string? url;

        var info = await signInManager.GetExternalLoginInfoAsync() ?? throw new BadRequestException();

        try
        {
            var email = info.Principal.GetEmail();
            var phoneNumber = info.Principal.Claims.FirstOrDefault(c => c.Type is ClaimTypes.HomePhone or ClaimTypes.MobilePhone or ClaimTypes.OtherPhone)?.Value;

            var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user is null && (string.IsNullOrEmpty(email) is false || string.IsNullOrEmpty(phoneNumber) is false))
            {
                user = await userManager.FindUserAsync(new() { Email = email, PhoneNumber = phoneNumber });
            }

            if (user is null)
            {
                // Instead of automatically creating a user here, you can navigate to the sign-up page and pass the email and phone number in the query string.

                user = new() { LockoutEnabled = true };

                await userStore.SetUserNameAsync(user, Guid.NewGuid().ToString(), cancellationToken);

                if (string.IsNullOrEmpty(email) is false)
                {
                    await ((IUserEmailStore<User>)userStore).SetEmailAsync(user, email, cancellationToken);
                }

                if (string.IsNullOrEmpty(phoneNumber) is false)
                {
                    await ((IUserPhoneNumberStore<User>)userStore).SetPhoneNumberAsync(user, phoneNumber!, cancellationToken);
                }

                var result = await userManager.CreateAsync(user, password: Guid.NewGuid().ToString("N") /* Users can reset their password later. */);

                if (result.Succeeded is false)
                {
                    throw new BadRequestException(string.Join(", ", result.Errors.Select(e => new LocalizedString(e.Code, e.Description))));
                }

                await userManager.AddLoginAsync(user, info);
            }

            if (string.IsNullOrEmpty(email) is false && email == user.Email && await userManager.IsEmailConfirmedAsync(user) is false)
            {
                await ((IUserEmailStore<User>)userStore).SetEmailConfirmedAsync(user, true, cancellationToken);
                await userManager.UpdateAsync(user);
            }

            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == user.PhoneNumber && await userManager.IsPhoneNumberConfirmedAsync(user) is false)
            {
                await ((IUserPhoneNumberStore<User>)userStore).SetPhoneNumberConfirmedAsync(user, true, cancellationToken);
                await userManager.UpdateAsync(user);
            }

            (_, url) = await GenerateOtpTokenData(user, returnUrl); // Sign in with a magic link, and 2FA will be prompted if already enabled.
        }
        catch (Exception exp)
        {
            LogSocialSignInCallbackFailed(logger, exp, info.LoginProvider, info.Principal.GetDisplayName());
            url = $"sign-in?error={Uri.EscapeDataString(exp is KnownException ? Localizer[exp.Message] : Localizer[nameof(AppStrings.UnknownException)])}";
        }
        finally
        {
            await Request.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // We'll handle sign-in with the following redirects, so no external identity cookie is needed.
        }

        if (localHttpPort is not null) return Redirect(new Uri(new Uri($"http://localhost:{localHttpPort}/"), url).ToString());
        if (string.IsNullOrEmpty(AppSettings.WebClientUrl) is false) return Redirect(new Uri(new Uri(AppSettings.WebClientUrl), url).ToString());
        return LocalRedirect($"~/{url}");
    }

    [HttpGet]
    public async Task<ActionResult> SocialSignedIn()
    {
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                    (await htmlRenderer.RenderComponentAsync<SocialSignedInPage>()).ToHtmlString());

        return Content(html, "text/html", System.Text.Encoding.UTF8);
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to perform {loginProvider} social sign in for {principal}")]
    private static partial void LogSocialSignInCallbackFailed(ILogger logger, Exception exp, string loginProvider, string principal);

    private async Task<(string token, string url)> GenerateOtpTokenData(User user, string? returnUrl)
    {
        user.OtpRequestedOn = DateTimeOffset.Now;

        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded is false)
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());

        var token = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, FormattableString.Invariant($"Otp,{user.OtpRequestedOn}"));

        var qs = $"userName={Uri.EscapeDataString(user.UserName!)}";

        if (string.IsNullOrEmpty(returnUrl) is false)
        {
            qs += $"&return-url={Uri.EscapeDataString(returnUrl)}";
        }

        var url = $"sign-in?otp={Uri.EscapeDataString(token)}&{qs}&culture={CultureInfo.CurrentUICulture.Name}";

        return (token, url);
    }

    private async Task SendConfirmEmailToken(User user, CancellationToken cancellationToken)
    {
        var resendDelay = (DateTimeOffset.Now - user.EmailTokenRequestedOn) - AppSettings.Identity.EmailTokenRequestResendDelay;

        if (resendDelay < TimeSpan.Zero)
            throw new TooManyRequestsExceptions(Localizer[nameof(AppStrings.WaitForEmailTokenRequestResendDelay), resendDelay.Value.ToString("mm\\:ss")]);

        user.EmailTokenRequestedOn = DateTimeOffset.Now;
        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded is false)
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());

        var email = user.Email!;
        var token = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, FormattableString.Invariant($"VerifyEmail:{email},{user.EmailTokenRequestedOn}"));
        var link = new Uri(HttpContext.Request.GetWebClientUrl(), $"confirm?email={Uri.EscapeDataString(email)}&emailToken={Uri.EscapeDataString(token)}&culture={CultureInfo.CurrentUICulture.Name}");

        await emailService.SendEmailToken(user, email, token, link, cancellationToken);
    }

    private async Task SendConfirmPhoneToken(User user, CancellationToken cancellationToken)
    {
        var resendDelay = (DateTimeOffset.Now - user.PhoneNumberTokenRequestedOn) - AppSettings.Identity.PhoneNumberTokenRequestResendDelay;

        if (resendDelay < TimeSpan.Zero)
            throw new TooManyRequestsExceptions(Localizer[nameof(AppStrings.WaitForPhoneNumberTokenRequestResendDelay), resendDelay.Value.ToString("mm\\:ss")]);

        user.PhoneNumberTokenRequestedOn = DateTimeOffset.Now;
        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded is false)
            throw new ResourceValidationException(result.Errors.Select(e => new LocalizedString(e.Code, e.Description)).ToArray());

        var phoneNumber = user.PhoneNumber!;
        var token = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, FormattableString.Invariant($"VerifyPhoneNumber:{phoneNumber},{user.PhoneNumberTokenRequestedOn}"));
        var link = new Uri(HttpContext.Request.GetWebClientUrl(), $"confirm?phoneNumber={Uri.EscapeDataString(phoneNumber!)}&phoneToken={Uri.EscapeDataString(token)}&culture={CultureInfo.CurrentUICulture.Name}");

        await smsService.SendSms(Localizer[nameof(AppStrings.ConfirmPhoneTokenSmsText), token], phoneNumber, cancellationToken);
    }
}
