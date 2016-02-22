using System;
using System.Collections.Generic;
namespace Proto
{
	public class ProtoDic
	{
		public static List<int> _protoId = new List<int>() 
		{
			1001,
			1002,
		};
		public static List<Type> _protoType = new List<Type>() 
		{
			typeof(Login),
			typeof(ChatLogin),
		};
		public static Type GetProtoTypeByProtoId(int protoId) 
		{
			int index = _protoId.IndexOf(protoId);
			return _protoType[index];
		}
		public static int GetProtoIdByProtoType(Type type) 
		{
			int index = _protoType.IndexOf(type);
			return _protoId[index];
		}
		public static bool ContainProtoId(int protoId) 
		{
			if(_protoId.Contains(protoId))
			{
				return true;
			}
			return false;
		}
		public static bool ContainProtoType(Type type) 
		{
			if(_protoType.Contains(type))
			{
				return true;
			}
			return false;
		}
	}
}
