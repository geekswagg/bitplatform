﻿using Microsoft.AspNetCore.Components.Web;

namespace Bit.BlazorUI.Demo.Client.Core.Pages.Components.Inputs.OtpInput;

public partial class BitOtpInputDemo
{
    private readonly List<ComponentParameter> componentParameters =
    [
        new()
        {
            Name = "AutoFocus",
            Type = "bool",
            DefaultValue = "false",
            Description = "If true, the first input is auto focused.",
        },
        new()
        {
            Name = "Classes",
            Type = "BitOtpInputClassStyles?",
            DefaultValue = "null",
            Description = "Custom CSS classes for different parts of the BitOtpInput.",
            LinkType = LinkType.Link,
            Href = "#otpinput-class-styles",
        },
        new()
        {
            Name = "InputType",
            Type = "BitOtpInputType",
            DefaultValue = "BitOtpInputType.Text",
            Description = "Type of the inputs.",
            LinkType = LinkType.Link,
            Href = "#inputType-enum",
        },
        new()
        {
            Name = "Length",
            Type = "int",
            DefaultValue = "5",
            Description = "Length of the OTP or number of the inputs.",
        },
        new()
        {
            Name = "OnFill",
            Type = "EventCallback<string?>",
            Description = "Callback for when all of the inputs are filled.",
        },
        new()
        {
            Name = "OnFocusIn",
            Type = "EventCallback<FocusEventArgs>",
            Description = "onfocusin event callback for each input.",
        },
        new()
        {
            Name = "OnFocusOut",
            Type = "EventCallback<FocusEventArgs>",
            Description = "onfocusout event callback for each input.",
        },
        new()
        {
            Name = "OnInput",
            Type = "EventCallback<ChangeEventArgs>",
            Description = "oninput event callback for each input.",
        },
        new()
        {
            Name = "OnKeyDown",
            Type = "EventCallback<KeyboardEventArgs>",
            Description = "onkeydown event callback for each input.",
        },
        new()
        {
            Name = "OnPaste",
            Type = "EventCallback<ClipboardEventArgs>",
            Description = "onpaste event callback for each input.",
        },
        new()
        {
            Name = "Reversed",
            Type = "bool",
            DefaultValue = "false",
            Description = "Defines whether to render inputs in the opposite direction.",
        },
        new()
        {
            Name = "Styles",
            Type = "BitOtpInputClassStyles?",
            DefaultValue = "null",
            Description = "Custom CSS styles for different parts of the BitOtpInput.",
            LinkType = LinkType.Link,
            Href = "#otpinput-class-styles",
        },
        new()
        {
            Name = "Vertical",
            Type = "bool",
            DefaultValue = "false",
            Description = "Defines whether to render inputs vertically.",
        },
    ];
    private readonly List<ComponentSubClass> componentSubClasses =
    [
        new()
        {
            Id = "otpinput-class-styles",
            Title = "BitOtpInputClassStyles",
            Description = "",
            Parameters = new()
            {
                new()
                {
                    Name = "Root",
                    Type = "string?",
                    DefaultValue = "null",
                    Description = "Custom CSS classes/styles for the root element of the otp input.",
                },
                new()
                {
                    Name = "Input",
                    Type = "string?",
                    DefaultValue = "null",
                    Description = "Custom CSS classes/styles for each input in otp input.",
                }
            }
        }
    ];
    private readonly List<ComponentSubEnum> componentSubEnums =
    [
        new()
        {
            Id = "inputType-enum",
            Name = "BitOtpInputType",
            Items = new()
            {
                new()
                {
                    Name = "Text",
                    Description = "The OtpInput characters are shown as text.",
                    Value = "0"
                },
                new()
                {
                    Name = "Password",
                    Description = "The OtpInput characters are masked.",
                    Value = "1"
                },
                new()
                {
                    Name = "Number",
                    Description = "The OtpInput characters are number.",
                    Value = "2"
                }
            }
        }
    ];
    private readonly List<ComponentParameter> componentPublicMembers =
    [
        new()
        {
            Name = "InputElements",
            Type = "ElementReference[]",
            Description = "The ElementReferences to the input elements of the BitOtpInput.",
        },
        new()
        {
            Name = "FocusAsync",
            Type = "ValueTask",
            Description = "Gives focus to a specific input element of the BitOtpInput.",
        }
    ];


    private string? oneWayValue;
    private string? twoWayValue;

    private string? onChangeValue;
    private string? onFillValue;
    private (FocusEventArgs Event, int Index)? onFocusInArgs;
    private (FocusEventArgs Event, int Index)? onFocusOutArgs;
    private (ChangeEventArgs Event, int Index)? onInputArgs;
    private (KeyboardEventArgs Event, int Index)? onKeyDownArgs;
    private (ClipboardEventArgs Event, int Index)? onPasteArgs;

    private ValidationOtpInputModel validationOtpInputModel = new();
    public bool formIsValidSubmit;
    private async Task HandleValidSubmit()
    {
        formIsValidSubmit = true;

        await Task.Delay(3000);

        formIsValidSubmit = false;

        StateHasChanged();
    }

    private void HandleInvalidSubmit()
    {
        formIsValidSubmit = false;
    }



    private readonly string example1RazorCode = @"
<BitOtpInput />
<BitOtpInput Length=""4"" />
<BitOtpInput IsEnabled=""false"" />
<BitOtpInput AutoFocus=""true"" />";

    private readonly string example2RazorCode = @"
<BitOtpInput InputType=""BitOtpInputType.Text"" />
<BitOtpInput InputType=""BitOtpInputType.Number"" />
<BitOtpInput InputType=""BitOtpInputType.Password"" />";

    private readonly string example3RazorCode = @"
<BitOtpInput />
<BitOtpInput Reversed />
<BitOtpInput Vertical />
<BitOtpInput Vertical Reversed />";

    private readonly string example4RazorCode = @"
<style>
    .custom-class {
        padding: 1rem;
        max-width: max-content;
        background-color: lightskyblue;
    }

    .custom-input {
        border-radius: 50%;
        border: 1px solid red;
        box-shadow: tomato 0 0 1rem;
    }
</style>

<BitOtpInput Style=""box-shadow:aqua 0 0 0.5rem;max-width:max-content;"" />
<BitOtpInput Class=""custom-class"" />

<BitOtpInput Styles=""@(new() { Input = ""padding:0.5rem;background-color:goldenrod""})"" />
<BitOtpInput Classes=""@(new() { Input = ""custom-input""})"" />";

    private readonly string example5RazorCode = @"
<BitOtpInput Value=""@oneWayValue"" />
<BitTextField Style=""margin-top: 5px;"" @bind-Value=""oneWayValue"" />

<BitOtpInput @bind-Value=""twoWayValue"" />
<BitTextField Style=""margin-top: 5px;"" @bind-Value=""twoWayValue"" />";
    private readonly string example5CsharpCode = @"
private string? oneWayValue;
private string? twoWayValue;";

    private readonly string example6RazorCode = @"
<BitOtpInput OnChange=""v => onChangeValue = v"" />
<div>OnChange value: @onChangeValue</div>

<BitOtpInput OnFill=""v => onFillValue = v"" />
<div>OnFill value: @onFillValue</div>

<BitOtpInput OnFocusIn=""args => onFocusInArgs = args"" />
<div>Focus type: @onFocusInArgs?.Event.Type</div>
<div>Input index: @onFocusInArgs?.Index</div>

<BitOtpInput OnFocusOut=""args => onFocusOutArgs = args"" />
<div>Focus type: @onFocusOutArgs?.Event.Type</div>
<div>Input index: @onFocusOutArgs?.Index</div>

<BitOtpInput OnInput=""args => onInputArgs = args"" />
<div>Value: @onInputArgs?.Event.Value</div>
<div>Input index: @onInputArgs?.Index</div>

<BitOtpInput OnKeyDown=""args => onKeyDownArgs = args"" />
<div>Key & Code: [@onKeyDownArgs?.Event.Key] [@onKeyDownArgs?.Event.Code]</div>
<div>Input index: @onKeyDownArgs?.Index</div>

<BitOtpInput OnPaste=""args => onPasteArgs = args"" />
<div>Focus type: @onPasteArgs?.Event.Type</div>
<div>Input index: @onPasteArgs?.Index</div>";
    private readonly string example6CsharpCode = @"
private string? onChangeValue;
private string? onFillValue;
private (FocusEventArgs Event, int Index)? onFocusInArgs;
private (FocusEventArgs Event, int Index)? onFocusOutArgs;
private (ChangeEventArgs Event, int Index)? onInputArgs;
private (KeyboardEventArgs Event, int Index)? onKeyDownArgs;
private (ClipboardEventArgs Event, int Index)? onPasteArgs;";

    private readonly string example7RazorCode = @"
<style>
    .validation-message {
        color: red;
    }
</style>

<EditForm Model=""validationOtpInputModel"" OnValidSubmit=""HandleValidSubmit"" OnInvalidSubmit=""HandleInvalidSubmit"">
    <DataAnnotationsValidator />

    <BitOtpInput Length=""6"" @bind-Value=""validationOtpInputModel.OtpValue"" />
    <ValidationMessage For=""() => validationOtpInputModel.OtpValue"" />

    <BitButton Style=""margin-top: 10px;"" ButtonType=""BitButtonType.Submit"">Submit</BitButton>
</EditForm>";
    private readonly string example7CsharpCode = @"
public class ValidationOtpInputModel
{
    [Required(ErrorMessage = ""The OTP value is required."")]
    [MinLength(6, ErrorMessage = ""Minimum length is 6."")]
    public string OtpValue { get; set; }
}

private ValidationOtpInputModel validationOtpInputModel = new();

private void HandleValidSubmit() { }
private void HandleInvalidSubmit() { }";

    private readonly string example8RazorCode = @"
<BitOtpInput Dir=""BitDir.Rtl"" />
<BitOtpInput Reversed Dir=""BitDir.Rtl"" />
<BitOtpInput Vertical Dir=""BitDir.Rtl"" />
<BitOtpInput Vertical Reversed Dir=""BitDir.Rtl"" />";
}
