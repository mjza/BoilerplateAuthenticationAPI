// Dummy class to group shared resources for localization

namespace WebApi
{
    public static class SharedResource
    {
        public const string PasswordRegEx = @"^(?=.*[a-zäöü])(?=.*[A-ZÄÖÜß])(?=.*\d)[a-zäöüA-ZÄÖÜß0-9\s!@#$%^&*§\/\? '+=)(<>;,.:_°`´-]{8,30}$";
    }
}
