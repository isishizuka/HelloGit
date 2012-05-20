using System;
using System.Collections.Generic;
using System.Linq;


using System.Reflection;

namespace TrimStringTest
{
	/// <summary>
	/// テストデータ
	/// </summary>
	struct A
	{
		String a;
		public A(String a)
		{
			this.a = a;
		}
	};
	/// <summary>
	/// テストデータ
	/// </summary>
	class B
	{
		int b = 123;
		string c = " c ";
		String d = " d";
	};
	/// <summary>
	/// テストデータ
	/// </summary>
	class C
	{
		A e = new A(" e");
		B[] f = new B[3] { new B(), new B(), new B() };
		B g = new B();
		string h = " h";
		int i = 123;
	};

	class Program
	{
		static IEnumerable<Tuple<object, FieldInfo>> EnumTargetFieldInfo(object obj)
		{
			return obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.SelectMany(f => f.FieldType != typeof(string) && typeof(IEnumerable<object>).IsAssignableFrom(f.FieldType)
					? ((IEnumerable<object>)f.GetValue(obj)).SelectMany(EnumTargetFieldInfo)
					: EnumerableEx.Return(Tuple.Create(obj, f))
				);
		}
		public static IEnumerable<Object> EnumTargetTypes(object obj)
		{
			return EnumTargetFieldInfo(obj).Select(obj_field => obj_field.Item2.GetValue(obj_field.Item1));
		}

		public static IEnumerable<T> EnumTargetTypes<T>(object obj)
		{
			return EnumTargetTypes(obj).OfType<T>();
		}

		public static void ActionTargetType<T>(object obj, Func<T, T> fun)
		{
			EnumTargetFieldInfo(obj).ForEach(obj_field=>
				{
					if (typeof(T).IsAssignableFrom(obj_field.Item2.FieldType))
					{
						var v = obj_field.Item2.GetValue(obj_field.Item1);
						obj_field.Item2.SetValue(obj_field.Item1, fun((T)v));
					}
				});
		}

		/// <summary>
		/// Main()
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			var c = new C();
			foreach (var str in EnumTargetTypes<String>(c))
			{
				Console.WriteLine("{0}", str);
			}
			ActionTargetType<String>(c, member => member.Trim());
			foreach (var str in EnumTargetTypes<String>(c))
			{
				Console.WriteLine("{0}", str);
			}
		}
	}
}
