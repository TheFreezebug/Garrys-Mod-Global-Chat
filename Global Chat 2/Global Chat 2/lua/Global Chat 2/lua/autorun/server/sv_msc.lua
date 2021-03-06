require("glsock2");


if GlobalChat then
	if GlobalChat.Sock then
		GlobalChat.Sock:Close()
	end
 
end 
 
GlobalChat = {}
GlobalChat.Server = "ip addr"
GlobalChat.Port = 7845
GlobalChat.ServerID = "server name"
GlobalChat.IDNO = 0xA1
GlobalChat.Password = "password"

GlobalChat.ServerCodes = {
	AUTH_REQUEST = 0x20, 
	SERVER_FULL = 0x34,
	AUTH_SUCCESS = 0x4, 
	FAIL_AUTH = 0x5,

 

}
 


util.AddNetworkString("GlobalChatText")

local function cout(x) 
	Msg("[") MsgC(Color(255,0,255,255),"GlobalChat") Msg("]: ") MsgN(x)
end 


GLSOCK_SUCCESS = GLSOCK_ERROR_SUCCESS
GlobalChat.Sock = GLSock(GLSOCK_TYPE_TCP)
 
 
function GlobalChat.Connect(sck,err) 
	
	if (err==GLSOCK_SUCCESS)  then
		// Send initial buffer to ID server.
		local buffer = GLSockBuffer()
		buffer:Write(GlobalChat.Password)
		sck:Send(buffer,function()  //send buffer to server with callback of cout success.
			cout("Connection SUCCESS.")
		end)
		GlobalChat.SendServerMessage("Server connected.") 
		sck:Read(1000,GlobalChat.Read)//Read with callback of GlobalChat.Read.
	else
		//cout("Something went wrong, terminating scoket. ughh")
		//sck:Close()
		
		//cout("Attempting reconnect in 15 seconds.") 
		//timer.Simple(15,function()
		//	GlobalChat.Sock:Connect(GlobalChat.Server,GlobalChat.Port,GlobalChat.Connect)

		//end)
	end 
end
  

function GlobalChat.DataHandler(str)

	if #str < 10 then 
		local bt = string.byte(str)
		if bt == GlobalChat.ServerCodes.AUTH_REQUEST then 
				cout("0x20 - Server requests authentication. ")
				return 
		end

		if bt == GlobalChat.ServerCodes.AUTH_SUCCESS then 
				cout("0x4 - Authentication was successful ")
				return 
		end
		

		if bt == GlobalChat.ServerCodes.FAIL_AUTH then 
				cout("0x5 - Bad authentication key ")
				return 
		end
	local ad = hook.Run("GlobalChatMessage",bufftab)


		return 
	end

	if !str[1]=="{" then return end // json check 

	local buftab = util.JSONToTable(str)
	local ad = hook.Run("GlobalChatMessage",bufftab)

	if ad then return false end

	if buftab["Type"]=="GlobalChat" then 
		
		local server = buftab["ServerID"]
		local buff = buftab["BufferData"]

		local msg = {Color(255,255,255),"[",server,"]"," ",buff["teamcolor"],buff["name"],Color(255,255,255),": ",buff["message"]}




		for k,v in pairs(player.GetAll()) do 
			if v["DisableGlobalChat"]~=true then 
				
				net.Start("GlobalChatText")
					net.WriteTable(msg)
				net.Send(v)
			end
		end



	end

	if buftab["Type"]=="ServerMessage" then 
		local server = buftab["ServerID"]
		local buff = buftab["BufferData"]

		local msg = {Color(255,0,255),"[",server,"]",Color(255,255,0)," : ", buff["message"]}



		for k,v in pairs(player.GetAll()) do 

				net.Start("GlobalChatText")
					net.WriteTable(msg)
				net.Send(v)
		
		end



	end



	if buftab["Type"]=="CServMessage" then 
	
		local buff = buftab["BufferData"]

		local msg = {Color(150,150,150),"[ChatServer]",Color(255,255,255)," : ", buff["message"]}



		for k,v in pairs(player.GetAll()) do 
			
				net.Start("GlobalChatText")
					net.WriteTable(msg)
				net.Send(v)
		
		end



	end


end
function GlobalChat.Chat(plr,t) 
	local tbrk = string.Explode(" ",t)
	local sck = GlobalChat.Sock

	if tbrk[1]=="!global" then
		if tbrk[2]=="on" then 


				local msg = {Color(150,150,150),"[GlobalChat]: ",Color(255,255,255), "Global chat is now ON."}
			
				net.Start("GlobalChatText")
					net.WriteTable(msg)
				net.Send(plr)
		
			plr["DisableGlobalChat"] = false 
		end
		if tbrk[2]=="off" then 

				local msg = {Color(150,150,150),"[GlobalChat]: ",Color(255,255,255), "Global chat is now OFF."}
			
				net.Start("GlobalChatText")
					net.WriteTable(msg)
				net.Send(plr)
		


			plr["DisableGlobalChat"] = true 
		end
		return ""
	end

	if plr["DisableGlobalChat"]~=true then 
		GlobalChat.SendBuffer(PlayerInfoJson(plr,t),"GlobalChat")
	end
	
end
hook.Add("PlayerSay","MultiChat",GlobalChat.Chat)

function GlobalChat.SendServerMessage(msg) 
	GlobalChat.SendBuffer({message = msg},"ServerMessage")

end

function PlayerInfoJson(ply,msgstr,mergtable) 
	if !IsValid(ply) then return false end
	local pinfo = {}	
	pinfo["name"]	 = ply:Nick()
	pinfo["steamid"] = ply:SteamID()
	pinfo["steamid64"] = ply:SteamID64()	
	pinfo["team"] = team.GetName(ply:Team())
	pinfo["teamcolor"] = team.GetColor(ply:Team())
	pinfo["message"] = msgstr
	pinfo["ugrp"] = ply:GetUserGroup();
	pinfo["gdisabled"] = ply["DisableGlobalChat"]==true  // spooky
	if !mergtable then mergtable = {} end
	for k,v in pairs(mergtable) do
		pinfo[k]=v
	end
	return pinfo
end


function GlobalChat.SendBuffer(tab,identity) 
	local buff = {}
	buff["ServerID"] = GlobalChat.ServerID
	buff["IDN"] = GlobalChat.IDNO
	buff["Type"] = identity 
	buff["BufferData"] = tab

	local bufftext = util.TableToJSON(buff)

	local sck = GlobalChat.Sock


		local buffer = GLSockBuffer()
		buffer:Write(bufftext)
		sck:Send(buffer,function()  //send buffer to server with callback of cout success.
			
		end)


 

end

 
 


function GlobalChat.Read(sck, buf , err) // read the socket buffer

	if (err == GLSOCK_SUCCESS) then
		local c,data = buf:Read(buf:Size()) // read the complete buffer.
		if (c > 0) then 
			local s,e = pcall(function() 
				GlobalChat.DataHandler(data) // Pass the (valid?) data on to the handler
			end)
			if !s then 
				print(e)
			end
		end
		sck:Read(1000,GlobalChat.Read) // Keep reading.
	else
		cout("Something went wrong, terminating scoket. nngh")
		sck:Close()
		cout("Attempting reconnect in 15 seconds.")
		timer.Simple(15,function()
			GlobalChat.Sock:Connect(GlobalChat.Server,GlobalChat.Port,GlobalChat.Connect)
		end)
	end 
end
GlobalChat.Sock:Connect(GlobalChat.Server,GlobalChat.Port,GlobalChat.Connect) 