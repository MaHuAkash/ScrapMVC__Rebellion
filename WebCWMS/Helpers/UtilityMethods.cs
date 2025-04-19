namespace WebCWMS.Helpers
{
	public static class UtilityMethods
	{
		public static byte[] GetDefaultProfilePicture(string webRootPath)
		{
			var imagePath = Path.Combine(webRootPath, "F1.jpg");
			return System.IO.File.ReadAllBytes(imagePath);
		}
	}
}
