using ServerCore;
using System.Net;
using System.Text;
using System;
using System.Collections.Generic;

public enum PacketID
{
    C_RequestEnterGame = 1,
	S_AnswerEnterGame = 2,
	C_SuccessToEnterServer = 3,
	C_LeaveGame = 4,
	S_BroadcastLeaveGame = 5,
	S_EnterSyncInfos = 6,
	C_CharacterInfo = 7,
	S_BroadcastCharacterInfo = 8,
	C_Damage = 9,
	S_BroadcastDamage = 10,
	C_GenerateCharacter = 11,
	S_BroadcastGenerateCharacter = 12,
	C_RemoveCharacter = 13,
	S_BroadcastRemoveCharacter = 14,
	
}

public interface IPacket
{
	ushort Protocol { get;}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}
    

public class C_RequestEnterGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_RequestEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestEnterGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_AnswerEnterGame : IPacket
{
    public bool permission;
	public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.S_AnswerEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.permission = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_AnswerEnterGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(permission), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(playerId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_SuccessToEnterServer : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_SuccessToEnterServer; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_SuccessToEnterServer), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_LeaveGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastLeaveGame : IPacket
{
    public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastLeaveGame), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(playerId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_EnterSyncInfos : IPacket
{
    public class CharacterInfo
	{
	    public int characterId;
		public string characterName;
		public float posX;
		public float posY;
		public float posZ;
		public float xSpeed;
		public float ySpeed;
		public int characterState;
		public float characterMoveDirection;
		public int attackType;
		public bool isAttacking;
		public bool isJumping;
		public bool isContactGround;
		public bool isConnectCombo;
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			ushort characterNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.characterName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, characterNameLen);
			count += characterNameLen;
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.xSpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.ySpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.characterState = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.characterMoveDirection = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.attackType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.isAttacking = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
			this.isJumping = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
			this.isContactGround = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
			this.isConnectCombo = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			ushort characterNameLen = (ushort)Encoding.Unicode.GetBytes(this.characterName,0,characterName.Length,segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(characterNameLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += characterNameLen;
			
			Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(xSpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(ySpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(characterState), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(characterMoveDirection), 0,segment.Array ,segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(attackType), 0,segment.Array ,segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(isAttacking), 0,segment.Array ,segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			Array.Copy(BitConverter.GetBytes(isJumping), 0,segment.Array ,segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			Array.Copy(BitConverter.GetBytes(isContactGround), 0,segment.Array ,segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			Array.Copy(BitConverter.GetBytes(isConnectCombo), 0,segment.Array ,segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
	        return success;
	    }
	}
	
	public List<CharacterInfo> characterInfos = new List<CharacterInfo>();
	

    public ushort Protocol { get { return (ushort)PacketID.S_EnterSyncInfos; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterInfos.Clear();
		ushort characterInfoLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(int i =0; i < characterInfoLen; i++)
		{
		    CharacterInfo characterInfo = new CharacterInfo();
		    characterInfo.Read(segment, ref count);
		    characterInfos.Add(characterInfo);
		}
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_EnterSyncInfos), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.characterInfos.Count), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (CharacterInfo characterInfo in characterInfos)
		    characterInfo.Write(segment,ref count);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_CharacterInfo : IPacket
{
    public int characterId;
	public float posX;
	public float posY;
	public float posZ;
	public float xSpeed;
	public float ySpeed;
	public int characterState;
	public float characterMoveDirection;
	public int attackType;
	public bool isAttacking;
	public bool isJumping;
	public bool isContactGround;
	public bool isConnectCombo;

    public ushort Protocol { get { return (ushort)PacketID.C_CharacterInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.xSpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.ySpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.characterState = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterMoveDirection = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.attackType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isAttacking = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isJumping = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isContactGround = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isConnectCombo = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_CharacterInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(xSpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(ySpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(characterState), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterMoveDirection), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(attackType), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isAttacking), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isJumping), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isContactGround), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isConnectCombo), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastCharacterInfo : IPacket
{
    public int characterId;
	public float posX;
	public float posY;
	public float posZ;
	public float xSpeed;
	public float ySpeed;
	public int characterState;
	public float characterMoveDirection;
	public int attackType;
	public bool isAttacking;
	public bool isJumping;
	public bool isContactGround;
	public bool isConnectCombo;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastCharacterInfo; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.xSpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.ySpeed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.characterState = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.characterMoveDirection = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.attackType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isAttacking = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isJumping = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isContactGround = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.isConnectCombo = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastCharacterInfo), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(xSpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(ySpeed), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(characterState), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(characterMoveDirection), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(attackType), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isAttacking), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isJumping), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isContactGround), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(isConnectCombo), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_Damage : IPacket
{
    public int characterId;
	public float directionX;
	public float directionY;
	public float power;
	public float stagger;

    public ushort Protocol { get { return (ushort)PacketID.C_Damage; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.directionX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.directionY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.power = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.stagger = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Damage), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(directionX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(directionY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(power), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(stagger), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastDamage : IPacket
{
    public int characterId;
	public float directionX;
	public float directionY;
	public float power;
	public float stagger;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastDamage; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.directionX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.directionY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.power = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.stagger = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastDamage), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(directionX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(directionY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(power), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(stagger), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_GenerateCharacter : IPacket
{
    public string characterName;
	public bool isPlayerCharacter;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.C_GenerateCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort characterNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.characterName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, characterNameLen);
		count += characterNameLen;
		
		this.isPlayerCharacter = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_GenerateCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        ushort characterNameLen = (ushort)Encoding.Unicode.GetBytes(this.characterName,0,characterName.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(characterNameLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += characterNameLen;
		
		Array.Copy(BitConverter.GetBytes(isPlayerCharacter), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastGenerateCharacter : IPacket
{
    public string characterName;
	public int characterId;
	public bool isPlayerCharacter;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastGenerateCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort characterNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.characterName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, characterNameLen);
		count += characterNameLen;
		
		this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.isPlayerCharacter = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastGenerateCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        ushort characterNameLen = (ushort)Encoding.Unicode.GetBytes(this.characterName,0,characterName.Length,segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(characterNameLen), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += characterNameLen;
		
		Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(isPlayerCharacter), 0,segment.Array ,segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		
		Array.Copy(BitConverter.GetBytes(posX), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posY), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(posZ), 0,segment.Array ,segment.Offset + count, sizeof(float));
		count += sizeof(float);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_RemoveCharacter : IPacket
{
    public int characterId;

    public ushort Protocol { get { return (ushort)PacketID.C_RemoveCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RemoveCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastRemoveCharacter : IPacket
{
    public int characterId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastRemoveCharacter; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.characterId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastRemoveCharacter), 0,segment.Array ,segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(characterId), 0,segment.Array ,segment.Offset + count, sizeof(int));
		count += sizeof(int);
		

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

