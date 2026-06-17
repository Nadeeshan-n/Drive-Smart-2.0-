# Copilot Instructions

## Project Guidelines
- In WPF forms with TextBox controls wrapped in Border elements, validation color feedback should be applied to the Border's background, not the TextBox's background, because TextBox styles often have Background="{x:Null}" which makes it transparent, hiding any background color changes applied directly to the TextBox. Use a helper method to access the parent Border via control.Parent is Border casting.