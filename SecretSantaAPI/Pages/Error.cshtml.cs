using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ErrorModel : PageModel
{
    public string ErrorMessage { get; private set; }

    public void OnGet(string errorCode)
    {
        ErrorMessage = errorCode switch
        {
            "AddUserFailed" => "Failed to add the user to the group. The user might already be a member or the group doesn't exist.",
            "InvalidToken" => "The invitation link is invalid or expired.",
            "GroupFull" => "This group is already at full capacity.",
            _ => "An unexpected error occurred."
        };
    }
}
