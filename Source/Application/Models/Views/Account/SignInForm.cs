namespace Application.Models.Views.Account
{
	public class SignInForm
	{
		#region Properties

		public virtual string ReturnUrl { get; set; }
		public virtual string Secret { get; set; }

		#endregion
	}
}