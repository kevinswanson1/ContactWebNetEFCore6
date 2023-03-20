using System.ComponentModel.DataAnnotations;

namespace ContactWebNetEFCore6Models
{
	//internal class Contact
	public class Contact
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "First name is required.")]
		[Display(Name = "First Name")]
		[StringLength(ContactManagerConstants.MAX_FIRST_NAME_LENGTH)]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last name is required.")]
		[Display(Name = "Last Name")]
		[StringLength(ContactManagerConstants.MAX_LAST_NAME_LENGTH)]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Email address is required.")]
		[Display(Name = "Email Address")]
		[StringLength(ContactManagerConstants.MAX_EMAIL_LENGTH)]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone number is required.")]
		[Display(Name = "Mobile phone")]
		[StringLength(ContactManagerConstants.MAX_PHONE_LENGTH)]
		[Phone(ErrorMessage = "Invalid phone number.")]
		public string PhonePrimary { get; set; }

		[Required(ErrorMessage = "Phone number is required.")]
		[Display(Name = "Home/Office phone")]
		[StringLength(ContactManagerConstants.MAX_PHONE_LENGTH)]
		[Phone(ErrorMessage = "Invalid phone number.")]
		public string PhoneSecondary { get; set; }

		[DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

		[Display(Name = "Streed Address Line 1")]
		[StringLength(ContactManagerConstants.MAX_STREET_ADDRESS_LENGTH)]
        public string StreetAddress1 { get; set; }

		[Display(Name = "Streed Address Line 2")]
		[StringLength(ContactManagerConstants.MAX_STREET_ADDRESS_LENGTH)]
		public string StreetAddress2 { get; set; }

		[Required(ErrorMessage = "City is required.")]
		[StringLength(ContactManagerConstants.MAX_CITY_LENGTH)]
		public string City { get; set; }

		// Reference to Id to State Model
		[Required(ErrorMessage = "State is required.")]
		[Display(Name = "State")]
		public int StateId { get; set; }

		public virtual State State { get; set; }
		// End Reference to Id to State Model

		[Required(ErrorMessage = "Zip code is required.")]
		[Display(Name = "Zip Code")]
		[StringLength(ContactManagerConstants.MAX_ZIP_CODE_LENGTH, MinimumLength = ContactManagerConstants.MIN_ZIP_CODE_LENGTH)]
		[RegularExpression("(^\\d{5}(-\\d{4})?$)|(^[ABCEGHJKLMNPRSTVXY]{1}\\d{1}[A-Z]{1} *\\d{1}[A-Z]{1}\\d{1}$)", ErrorMessage = "Zip code is invalid.")]
		public string Zip { get; set; }

		[Required(ErrorMessage = "The User ID is required in order to map the contact to a user correctly.")]
		public string UserId { get; set; }

		[Display(Name = "Full Name")]
		public string FreiendlyName => $"{FirstName} {LastName}";

		[Display(Name = "Address")]
		public string FreindlyAddress => 
			State is null 
				? ""
				: string.IsNullOrWhiteSpace(StreetAddress1) 
					? $"{City}, {State.Abbreviation}, {Zip}"
					: string.IsNullOrWhiteSpace(StreetAddress2) 
						? $"{StreetAddress1}, {City}, {State.Abbreviation}, {Zip}"
						: $"{StreetAddress1} - {StreetAddress2}, {City}, {State.Abbreviation}, {Zip}";
	}
}
