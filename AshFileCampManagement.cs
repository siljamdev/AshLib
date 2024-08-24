using System;

namespace AshLib;

public partial class AshFile
{
	public bool ExistsCamp(string name){
		if(this.data.ContainsKey(name)){
			return true;
		}
		return false;
	}
	
	//Set
	
	public void SetCamp(string name, CampValue val){
		this.data[name] = val;
	}
	
	public void SetCamp(string name, byte[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, string val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, byte val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, ushort val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, uint val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, ulong val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, sbyte val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, short val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, int val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, long val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, Color3 val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, float val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, double val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, Vec2 val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, Vec3 val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, Vec4 val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, bool val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, ushort[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, uint[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, ulong[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, sbyte[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, short[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, int[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, long[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, float[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, double[] val)
	{
		this.data[name] = new CampValue(val);
	}
	
	public void SetCamp(string name, Date val)
	{
		this.data[name] = new CampValue(val);
	}

	//Initialize
	
	public void InitializeCamp(string name, CampValue val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, val);
	}
	
	public void InitializeCamp(string name, byte[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, string val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, byte val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, ushort val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, uint val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, ulong val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, sbyte val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, short val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, int val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, long val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, Color3 val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, float val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, double val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, Vec2 val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, Vec3 val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, Vec4 val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, bool val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, ushort[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, uint[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, ulong[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, sbyte[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, short[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, int[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, long[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, float[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, double[] val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}
	
	public void InitializeCamp(string name, Date val){
		if(this.ExistsCamp(name)){
			return;
		}
		this.SetCamp(name, new CampValue(val));
	}

	//Get
	
	public CampValue GetCampValue(string name){
		if(!this.ExistsCamp(name)){
			return CampValue.Null;
		}
		return this.data[name];
	}
	
	public bool CanGetCampValue(string name, out CampValue val){
		if(!this.ExistsCamp(name)){
			val = CampValue.Null;
			return false;
		}
		val = this.data[name];
		return true;
	}
	
	public object GetCamp(string name){
		if(!this.ExistsCamp(name)){
			return null;
		}
		return this.data[name].GetValue();
	}
	
	public bool CanGetCamp(string name, out object val){
		if(!this.ExistsCamp(name)){
			val = null;
			return false;
		}
		val = this.data[name].GetValue();
		return true;
	}
	
	//can get as...
	
	public bool CanGetCampAsByteArray(string name, out byte[] val){
		if(!this.ExistsCamp(name)){
			val = (byte[])null;
			return false;
		}
		if(this.data[name].CanGetByteArray(out byte[] b)){
			val = b;
			return true;
		}
		val = (byte[])null;
		return false;
	}
	
	public bool CanGetCampAsString(string name, out string val){
		if(!this.ExistsCamp(name)){
			val = (string)null;
			return false;
		}
		if(this.data[name].CanGetString(out string s)){
			val = s;
			return true;
		}
		val = (string)null;
		return false;
	}
	
	public bool CanGetCampAsByte(string name, out byte val){
		if(!this.ExistsCamp(name)){
			val = (byte)0;
			return false;
		}
		if(this.data[name].CanGetByte(out byte b)){
			val = b;
			return true;
		}
		val = (byte)0;
		return false;
	}
	
	public bool CanGetCampAsUshort(string name, out ushort val){
		if(!this.ExistsCamp(name)){
			val = (ushort)0;
			return false;
		}
		if(this.data[name].CanGetUshort(out ushort us)){
			val = us;
			return true;
		}
		val = (ushort)0;
		return false;
	}
	
	public bool CanGetCampAsUint(string name, out uint val){
		if(!this.ExistsCamp(name)){
			val = (uint)0;
			return false;
		}
		if(this.data[name].CanGetUint(out uint ui)){
			val = ui;
			return true;
		}
		val = (uint)0;
		return false;
	}
	
	public bool CanGetCampAsUlong(string name, out ulong val){
		if(!this.ExistsCamp(name)){
			val = (ulong)0;
			return false;
		}
		if(this.data[name].CanGetUlong(out ulong ul)){
			val = ul;
			return true;
		}
		val = (ulong)0;
		return false;
	}
	
	public bool CanGetCampAsSbyte(string name, out sbyte val){
		if(!this.ExistsCamp(name)){
			val = (sbyte)0;
			return false;
		}
		if(this.data[name].CanGetSbyte(out sbyte sb)){
			val = sb;
			return true;
		}
		val = (sbyte)0;
		return false;
	}
	
	public bool CanGetCampAsShort(string name, out short val){
		if(!this.ExistsCamp(name)){
			val = (short)0;
			return false;
		}
		if(this.data[name].CanGetShort(out short s)){
			val = s;
			return true;
		}
		val = (short)0;
		return false;
	}
	
	public bool CanGetCampAsInt(string name, out int val){
		if(!this.ExistsCamp(name)){
			val = (int)0;
			return false;
		}
		if(this.data[name].CanGetInt(out int i)){
			val = i;
			return true;
		}
		val = (int)0;
		return false;
	}
	
	public bool CanGetCampAsLong(string name, out long val){
		if(!this.ExistsCamp(name)){
			val = (long)0;
			return false;
		}
		if(this.data[name].CanGetLong(out long l)){
			val = l;
			return true;
		}
		val = (long)0;
		return false;
	}
	
	public bool CanGetCampAsColor(string name, out Color3 val){
		if(!this.ExistsCamp(name)){
			val = (Color3)new Color3(0, 0, 0);
			return false;
		}
		if(this.data[name].CanGetColor(out Color3 c)){
			val = c;
			return true;
		}
		val = (Color3)new Color3(0, 0, 0);
		return false;
	}
	
	public bool CanGetCampAsFloat(string name, out float val){
		if(!this.ExistsCamp(name)){
			val = (float)0;
			return false;
		}
		if(this.data[name].CanGetFloat(out float f)){
			val = f;
			return true;
		}
		val = (float)0;
		return false;
	}
	
	public bool CanGetCampAsDouble(string name, out double val){
		if(!this.ExistsCamp(name)){
			val = (double)0;
			return false;
		}
		if(this.data[name].CanGetDouble(out double d)){
			val = d;
			return true;
		}
		val = (double)0;
		return false;
	}
	
	public bool CanGetCampAsVec2(string name, out Vec2 val){
		if(!this.ExistsCamp(name)){
			val = (Vec2)new Vec2(0f, 0f);
			return false;
		}
		if(this.data[name].CanGetVec2(out Vec2 v)){
			val = v;
			return true;
		}
		val = (Vec2)new Vec2(0f, 0f);
		return false;
	}
	
	public bool CanGetCampAsVec3(string name, out Vec3 val){
		if(!this.ExistsCamp(name)){
			val = (Vec3)new Vec3(0f, 0f, 0f);
			return false;
		}
		if(this.data[name].CanGetVec3(out Vec3 v)){
			val = v;
			return true;
		}
		val = (Vec3)new Vec3(0f, 0f, 0f);
		return false;
	}
	
	public bool CanGetCampAsVec4(string name, out Vec4 val){
		if(!this.ExistsCamp(name)){
			val = (Vec4)new Vec4(0f, 0f, 0f, 0f);
			return false;
		}
		if(this.data[name].CanGetVec4(out Vec4 v)){
			val = v;
			return true;
		}
		val = (Vec4)new Vec4(0f, 0f, 0f, 0f);
		return false;
	}
	
	public bool CanGetCampAsBool(string name, out bool val){
		if(!this.ExistsCamp(name)){
			val = (bool)false;
			return false;
		}
		if(this.data[name].CanGetBool(out bool b)){
			val = b;
			return true;
		}
		val = (bool)false;
		return false;
	}
	
	public bool CanGetCampAsUbyteArray(string name, out byte[] val){
		if(!this.ExistsCamp(name)){
			val = (byte[])null;
			return false;
		}
		if(this.data[name].CanGetUbyteArray(out byte[] b)){
			val = b;
			return true;
		}
		val = (byte[])null;
		return false;
	}
	
	public bool CanGetCampAsUshortArray(string name, out ushort[] val){
		if(!this.ExistsCamp(name)){
			val = (ushort[])null;
			return false;
		}
		if(this.data[name].CanGetUshortArray(out ushort[] us)){
			val = us;
			return true;
		}
		val = (ushort[])null;
		return false;
	}
	
	public bool CanGetCampAsUintArray(string name, out uint[] val){
		if(!this.ExistsCamp(name)){
			val = (uint[])null;
			return false;
		}
		if(this.data[name].CanGetUintArray(out uint[] ui)){
			val = ui;
			return true;
		}
		val = (uint[])null;
		return false;
	}
	
	public bool CanGetCampAsUlongArray(string name, out ulong[] val){
		if(!this.ExistsCamp(name)){
			val = (ulong[])null;
			return false;
		}
		if(this.data[name].CanGetUlongArray(out ulong[] ul)){
			val = ul;
			return true;
		}
		val = (ulong[])null;
		return false;
	}
	
	public bool CanGetCampAsSbyteArray(string name, out sbyte[] val){
		if(!this.ExistsCamp(name)){
			val = (sbyte[])null;
			return false;
		}
		if(this.data[name].CanGetSbyteArray(out sbyte[] sb)){
			val = sb;
			return true;
		}
		val = (sbyte[])null;
		return false;
	}
	
	public bool CanGetCampAsShortArray(string name, out short[] val){
		if(!this.ExistsCamp(name)){
			val = (short[])null;
			return false;
		}
		if(this.data[name].CanGetShortArray(out short[] s)){
			val = s;
			return true;
		}
		val = (short[])null;
		return false;
	}
	
	public bool CanGetCampAsIntArray(string name, out int[] val){
		if(!this.ExistsCamp(name)){
			val = (int[])null;
			return false;
		}
		if(this.data[name].CanGetIntArray(out int[] i)){
			val = i;
			return true;
		}
		val = (int[])null;
		return false;
	}
	
	public bool CanGetCampAsLongArray(string name, out long[] val){
		if(!this.ExistsCamp(name)){
			val = (long[])null;
			return false;
		}
		if(this.data[name].CanGetLongArray(out long[] l)){
			val = l;
			return true;
		}
		val = (long[])null;
		return false;
	}
	
	public bool CanGetCampAsFloatArray(string name, out float[] val){
		if(!this.ExistsCamp(name)){
			val = (float[])null;
			return false;
		}
		if(this.data[name].CanGetFloatArray(out float[] f)){
			val = f;
			return true;
		}
		val = (float[])null;
		return false;
	}
	
	public bool CanGetCampAsDoubleArray(string name, out double[] val){
		if(!this.ExistsCamp(name)){
			val = (double[])null;
			return false;
		}
		if(this.data[name].CanGetDoubleArray(out double[] d)){
			val = d;
			return true;
		}
		val = (double[])null;
		return false;
	}
	
	public bool CanGetCampAsDate(string name, out Date val){
		if(!this.ExistsCamp(name)){
			val = (Date)AshLib.Date.Invalid;
			return false;
		}
		if(this.data[name].CanGetDate(out Date d)){
			val = d;
			return true;
		}
		val = (Date)AshLib.Date.Invalid;
		return false;
	}
	
	//get type
	
	public Type GetCampType(string name){
		if(!this.ExistsCamp(name)){
			return (Type) (-1);
		}
		return this.data[name].type;
	}
	
	public bool CanGetCampType(string name, out Type t){
		if(!this.ExistsCamp(name)){
			t = (Type) (-1);
			return false;
		}
		t = this.data[name].type;
		return true;
	}
	
	//delete
	
	public void DeleteCamp(string name){
		if(!ExistsCamp(name)){
			return;
		}
		this.data.Remove(name);
	}
	
	public bool CanDeleteCamp(string name){
		if(!this.ExistsCamp(name)){
			return false;
		}
		this.data.Remove(name);
		return true;
	}
	
	//Rename
	
	public void RenameCamp(string oldName, string newName){
		if(!this.ExistsCamp(oldName)){
			return;
		}
		CampValue o = this.GetCampValue(oldName);
		this.DeleteCamp(oldName);
		this.SetCamp(newName, o);
	}
	
	public bool CanRenameCamp(string oldName, string newName){
		if(!this.ExistsCamp(oldName)){
			return false;
		}
		CampValue o = this.GetCampValue(oldName);
		this.DeleteCamp(oldName);
		this.SetCamp(newName, o);
		return true;
	}
}