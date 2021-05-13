namespace GodsEye.Application.Api.Messages
{
    public class UserSecrets
    {
        public string UserId { get; set; }

        public string SearchedPerson { get; set; }

        public void Deconstruct(out string userId, out string searchedPerson)
        {
            userId = UserId;
            searchedPerson = SearchedPerson;
        }
    }
}
