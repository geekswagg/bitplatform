﻿namespace Bit.BlazorUI.Demo.Client.Core.Pages.Components.Buttons;

public partial class BitToggleButtonDemo
{
    private readonly List<ComponentParameter> componentParameters =
    [
        new()
        {
            Name = "AllowDisabledFocus",
            Type = "bool",
            DefaultValue = "true",
            Description = "Whether the toggle button can have focus in disabled mode.",
        },
        new()
        {
            Name = "AriaDescription",
            Type = "string?",
            DefaultValue = "null",
            Description = "Detailed description of the toggle button for the benefit of screen readers.",
        },
        new()
        {
            Name = "AriaHidden",
            Type = "bool",
            DefaultValue = "false",
            Description = "If true, add an aria-hidden attribute instructing screen readers to ignore the element.",
        },
        new()
        {
            Name = "ChildContent",
            Type = "RenderFragment?",
            DefaultValue = "null",
            Description = "The content of BitToggleButton.",
        },
        new()
        {
            Name = "Classes",
            Type = "BitToggleButtonClassStyles?",
            DefaultValue = "null",
            Description = "Custom CSS classes for different parts of the component.",
            Href = "#class-styles",
            LinkType = LinkType.Link,
        },
        new()
        {
            Name = "DefaultIsChecked",
            Type = "bool?",
            DefaultValue = "null",
            Description = "Default value of the IsChecked.",
        },
        new()
        {
            Name = "IconName",
            Type = "string?",
            DefaultValue = "null",
            Description = "The icon that shows in the toggle button.",
        },
        new()
        {
            Name = "IsChecked",
            Type = "bool",
            DefaultValue = "false",
            Description = "Determine if the toggle button is in checked state, default is true.",
        },
        new()
        {
            Name = "OnChange",
            Type = "EventCallback<bool>",
            Description = "Callback that is called when the IsChecked value has changed.",
        },
        new()
        {
            Name = "OnClick",
            Type = "EventCallback<MouseEventArgs>",
            Description = "Callback for when the button clicked.",
        },
        new()
        {
            Name = "OffIconName",
            Type = "string?",
            Description = "The icon of the BitToggleButton when it is not checked.",
        },
        new()
        {
            Name = "OffText",
            Type = "string?",
            Description = "The text of the BitToggleButton when it is not checked.",
        },
        new()
        {
            Name = "OffTitle",
            Type = "string?",
            Description = "The title of the BitToggleButton when it is not checked.",
        },
        new()
        {
            Name = "OnIconName",
            Type = "string?",
            Description = "The icon of the BitToggleButton when it is checked.",
        },
        new()
        {
            Name = "OnText",
            Type = "string?",
            Description = "The text of the BitToggleButton when it is checked.",
        },
        new()
        {
            Name = "OnTitle",
            Type = "string?",
            Description = "The title of the BitToggleButton when it is checked.",
        },
        new()
        {
            Name = "Size",
            Type = "BitSize",
            DefaultValue = "null",
            Description = "The size of button, Possible values: Small | Medium | Large.",
            LinkType = LinkType.Link,
            Href = "#button-size-enum",
        },
        new()
        {
            Name = "Styles",
            Type = "BitToggleButtonClassStyles?",
            DefaultValue = "null",
            Description = "Custom CSS styles for different parts of the component.",
            Href = "#class-styles",
            LinkType = LinkType.Link,
        },
        new()
        {
            Name = "Text",
            Type = "string?",
            DefaultValue = "null",
            Description = "The text of the BitToggleButton.",
        },
        new()
        {
            Name = "Title",
            Type = "string?",
            DefaultValue = "null",
            Description = "The title to show when the mouse is placed on the toggle button.",
        },
        new()
        {
            Name = "Variant",
            Type = "BitVariant?",
            DefaultValue = "null",
            Description = "The visual variant of the toggle button.",
            LinkType = LinkType.Link,
            Href = "#variant-enum",
        },
    ];

    private readonly List<ComponentSubClass> componentSubClasses =
    [
        new()
        {
            Id = "class-styles",
            Title = "BitToggleButtonClassStyles",
            Parameters =
            [
               new()
               {
                   Name = "Root",
                   Type = "string?",
                   DefaultValue = "null",
                   Description = "Custom CSS classes/styles for the root element of the BitToggleButton.",
               },
               new()
               {
                   Name = "Checked",
                   Type = "string?",
                   DefaultValue = "null",
                   Description = "Custom CSS classes/styles for the checked state of the BitToggleButton.",
               },
               new()
               {
                   Name = "Icon",
                   Type = "string?",
                   DefaultValue = "null",
                   Description = "Custom CSS classes/styles for the icon element of the BitToggleButton.",
               },
               new()
               {
                   Name = "Container",
                   Type = "string?",
                   DefaultValue = "null",
                   Description = "Custom CSS classes/styles for the icon and label container of the BitToggleButton.",
               },
               new()
               {
                   Name = "Text",
                   Type = "string?",
                   DefaultValue = "null",
                   Description = "Custom CSS classes/styles for the text element of the BitToggleButton.",
               }
            ]
        }
    ];

    private readonly List<ComponentSubEnum> componentSubEnums =
    [
        new()
        {
            Id = "variant-enum",
            Name = "BitVariant",
            Description = "Determines the variant of the content that controls the rendered style of the corresponding element(s).",
            Items =
            [
                new()
                {
                    Name= "Fill",
                    Description="Fill styled variant.",
                    Value="0",
                },
                new()
                {
                    Name= "Outline",
                    Description="Outline styled variant.",
                    Value="1",
                },
                new()
                {
                    Name= "Text",
                    Description="Text styled variant.",
                    Value="2",
                }
            ]
        },
        new()
        {
            Id = "button-size-enum",
            Name = "BitSize",
            Description = "",
            Items =
            [
                new()
                {
                    Name= "Small",
                    Description="The small size button.",
                    Value="0",
                },
                new()
                {
                    Name= "Medium",
                    Description="The medium size button.",
                    Value="1",
                },
                new()
                {
                    Name= "Large",
                    Description="The large size button.",
                    Value="2",
                }
            ]
        }
    ];



    private bool example51Value;
    private bool example52Value;



    private readonly string example1RazorCode = @"
<BitToggleButton OffText=""Fill Unmute"" OnText=""Fill Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Variant=""BitVariant.Fill"" />";

    private readonly string example2RazorCode = @"
<BitToggleButton OffText=""Fill Unmute"" OnText=""Fill Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Variant=""BitVariant.Fill"" />

<BitToggleButton OffText=""Outline Unmute"" OnText=""Outline Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Variant=""BitVariant.Outline"" />

<BitToggleButton OffText=""Text Unmute"" OnText=""Text Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Variant=""BitVariant.Text"" />";

    private readonly string example3RazorCode = @"
<BitToggleButton OffText=""Fill Unmute"" OnText=""Fill Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff"" />
<BitToggleButton IsEnabled=""false"" Text=""Disabled"" IconName=""@BitIconName.MicOff"" />";

    private readonly string example4RazorCode = @"
<BitToggleButton OffText=""Outline Unmute"" OnText=""Outline Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Variant=""BitVariant.Outline"" />
<BitToggleButton Variant=""BitVariant.Outline"" IsEnabled=""false"" Text=""Disabled"" IconName=""@BitIconName.MicOff"" />";

    private readonly string example5RazorCode = @"
<BitToggleButton OffText=""Text Unmute"" OnText=""Text Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Variant=""BitVariant.Text"" />
<BitToggleButton Variant=""BitVariant.Text"" IsEnabled=""false"" Text=""Disabled"" IconName=""@BitIconName.MicOff"" />";

    private readonly string example6RazorCode = @"
<BitToggleButton DefaultIsChecked=""true""
                 OffText=""Unmute"" OnText=""Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff"" />

<BitToggleButton @bind-IsChecked=""example51Value""
                 Text=""@(example51Value ? ""Mute"" : ""Unmute"")""
                 IconName=""@(example51Value ? BitIconName.MicOff : BitIconName.Microphone)"" />
<BitCheckbox Label=""Checked Toggle Button"" @bind-Value=""example51Value"" />

<BitToggleButton OnChange=""v => example52Value = v""
                 OffText=""Unmute"" OnText=""Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff"" />
<BitLabel>Check status is: @example52Value</BitLabel>";
    private readonly string example6CsharpCode = @"
private bool example51Value;
private bool example52Value;";

    private readonly string example7RazorCode = @"
<BitToggleButton Size=""BitSize.Small"" Variant=""BitVariant.Fill"">Small</BitToggleButton>
<BitToggleButton Size=""BitSize.Medium"" Variant=""BitVariant.Fill"">Medium</BitToggleButton>
<BitToggleButton Size=""BitSize.Large"" Variant=""BitVariant.Fill"">Large</BitToggleButton>

<BitToggleButton Size=""BitSize.Small"" Variant=""BitVariant.Outline"">Small</BitToggleButton>
<BitToggleButton Size=""BitSize.Medium"" Variant=""BitVariant.Outline"">Medium</BitToggleButton>
<BitToggleButton Size=""BitSize.Large"" Variant=""BitVariant.Outline"">Large</BitToggleButton>

<BitToggleButton Size=""BitSize.Small"" Variant=""BitVariant.Text"">Small</BitToggleButton>
<BitToggleButton Size=""BitSize.Medium"" Variant=""BitVariant.Text"">Medium</BitToggleButton>
<BitToggleButton Size=""BitSize.Large"" Variant=""BitVariant.Text"">Large</BitToggleButton>";

    private readonly string example8RazorCode = @"
<style>
    .custom-class {
        color: aqua;
        border-radius: 1rem;
    }

    .custom-container {
        font-size: 12px;
    }

    .custom-icon {
        color: blue;
    }

    .custom-text {
        color: red;
    }
</style>


<BitToggleButton Style=""color:darkblue; font-weight:bold""
                 OffText=""Styled Button : Unmute"" OnText=""Styled Button : Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff"" />

<BitToggleButton Class=""custom-class""
                 Variant=""BitVariant.Outline""
                 OffText=""Classed Button : Unmute"" OnText=""Classed Button : Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff"" />


<BitToggleButton OffText=""Styled Button : Unmute"" OnText=""Styled Button : Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Styles=""@(new() { Container = ""font-size: 18px;"", Icon = ""color: red;"", Text = ""color: blue;"" })"" />

<BitToggleButton Variant=""BitVariant.Outline""
                 OffText=""Classed Button : Unmute"" OnText=""Classed Button : Mute""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Classes=""@(new() { Container = ""custom-container"", Icon = ""custom-icon"", Text = ""custom-text"" })"" />";

    private readonly string example9RazorCode = @"
Visible: [ <BitToggleButton Visibility=""BitVisibility.Visible"">Visible toggle button</BitToggleButton> ]
Hidden: [ <BitToggleButton Visibility=""BitVisibility.Hidden"">Hidden toggle button</BitToggleButton> ]
Collapsed: [ <BitToggleButton Visibility=""BitVisibility.Collapsed"">Collapsed toggle button</BitToggleButton> ]";

    private readonly string example10RazorCode = @"
<BitToggleButton Dir=""BitDir.Rtl""
                 OffText=""صدا وصل"" OnText=""صدا قطع""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff"" />

<BitToggleButton Dir=""BitDir.Rtl""
                 OffText=""صدا وصل"" OnText=""صدا قطع""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Variant=""BitVariant.Outline"" />

<BitToggleButton Dir=""BitDir.Rtl""
                 OffText=""صدا وصل"" OnText=""صدا قطع""
                 OffIconName=""@BitIconName.Microphone"" OnIconName=""@BitIconName.MicOff""
                 Variant=""BitVariant.Text"" />";
}
