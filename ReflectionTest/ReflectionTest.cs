using System;
using System.Reflection;

namespace ReflectionTest
{
	public class ReflectionMain
	{
		//Delegate to store the method reference for later on
		delegate int PossiblyRandomNumberMethodDelegate();

		public static void Main()
		{
			//Reflection can be used to look into and modify objects.
			//It can also be used to create soft dependancies.

			//First, We'll just do some normal invocation
			Console.WriteLine ("==Normal invocation==");
			Console.WriteLine ("Facebook data: " + ObjectWeAreReflecting.GetFacebookData ());
			//We need to grab an instance of this to call instance methods
			ObjectWeAreReflecting reflectObject = new ObjectWeAreReflecting ();
			Console.WriteLine ("Possibly random number: " + reflectObject.GetPossiblyRandomNumber());
			Console.WriteLine ();


			//Secondly, Let's access a field. To do this, we need a FieldInfo of the field we want to access.
			Console.WriteLine ("==Field access==");
			FieldInfo facebookField = typeof(ObjectWeAreReflecting).GetField ("facebook", BindingFlags.Static | BindingFlags.Public);
			//Because it is static, GetValue does not need an 'ObjectWeAreReflecting' object to access the field. Call it with null for static fields.
			Console.WriteLine ("Facebook before: " + facebookField.GetValue (null));
			//Same with SetValue
			facebookField.SetValue (null, "Now we're not so sure we know everything");
			Console.WriteLine ("Facebook after: " + facebookField.GetValue (null));
			Console.WriteLine ();


			//Thirdly, Reflection can be used to access private fields and methods, which lets you do all type of whacky things.
			Console.WriteLine ("==Private member access==");
			//Let's grab a FieldInfo of ObjectWeAreReflecting.genitals
			FieldInfo genitalsField = typeof(ObjectWeAreReflecting).GetField ("genitals", BindingFlags.Instance | BindingFlags.NonPublic);
			//Because it is an instance field, GetValue needs an 'ObjectWeAreReflecting' object to access the field.
			//Let's use the reflectObject we created before.
			Console.WriteLine ("Genitals before: " + genitalsField.GetValue (reflectObject));
			//Same with SetValue
			genitalsField.SetValue (reflectObject, "Doesn't have crabs!");
			Console.WriteLine ("Genitals after: " + genitalsField.GetValue (reflectObject));
			Console.WriteLine ();


			//Forthly, You can use it to invoke methods, although it's quite slow. It's generally better to store it in a delegate.
			Console.WriteLine ("==Method access==");
			//First, grab the MethodInfo that we need
			MethodInfo possiblyRandomNumberMethod = typeof(ObjectWeAreReflecting).GetMethod ("GetPossiblyRandomNumber", BindingFlags.Instance | BindingFlags.Public);
			//Invoke the method
			Console.WriteLine ("Invoke possiblyRandomNumber: " + possiblyRandomNumberMethod.Invoke(reflectObject, null));
			//It's better to store it in a delegate due to speed
			PossiblyRandomNumberMethodDelegate storedMethod = (PossiblyRandomNumberMethodDelegate)Delegate.CreateDelegate (typeof(PossiblyRandomNumberMethodDelegate), reflectObject, possiblyRandomNumberMethod);
			Console.WriteLine ("Stored possiblyRandomNumber: " + storedMethod());
			Console.WriteLine ();


			//Lastly, we can also create soft dependancies by never referencing the assembly with the object, and treating it as type 'object'.
			//First, find the type of the object we need
			Console.WriteLine ("==Soft Dependancy==");
			Type unknownType = null;
			foreach (Assembly possibleAssembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type possibleType in possibleAssembly.GetExportedTypes())
				{
					if (possibleType.Name == "ObjectWeAreReflecting")
					{
						Console.WriteLine(possibleType.Name + " is the type we are looking for!");
						unknownType = possibleType;
					}
				}
			}
			//unknownObject is now a ObjectWeAreReflecting type.
			object unknownObject = Activator.CreateInstance(unknownType);
			FieldInfo unknownField = unknownType.GetField("genitals", BindingFlags.Instance | BindingFlags.NonPublic);
			Console.WriteLine("UknownObject's genitals: " + unknownField.GetValue(unknownObject));
			Console.WriteLine();
			Console.ReadKey();
		}

	}


	public class ObjectWeAreReflecting
	{
		//Information stored on facebook is public
		public static string facebook = "We know everything";

		//Gentials are also known as private parts
		private string genitals = "May or may not have crabs";

		public static void SetFacebookData(string newData)
		{
			facebook = newData;
		}

		public static string GetFacebookData()
		{
			return facebook;
		}

		public string GetPrivateData()
		{
			return genitals;
		}

		public int GetPossiblyRandomNumber()
		{
			return 4;
		}
	}
}

