require("glsock2");


if GlobalChat then
	if GlobalChat.Sock then
		GlobalChat.Sock:Close()
	end
 
end 

GlobalChat = {}
GlobalChat.Server = "108.241.117.61"
GlobalChat.Port = 27789
GlobalChat.ServerID = "DevTest"
GlobalChat.IDNO = 0xFF
GlobalChat.Password = "diabetes123"



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
	print(str)
	if #str < 10 then return false,"bad buffer." end
	local buftab = util.JSONToTable(str)
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


end
function GlobalChat.Chat(plr,t) 
	local sck = GlobalChat.Sock

	GlobalChat.SendBuffer(PlayerInfoJson(plr,t),"GlobalChat")
	
end
hook.Add("PlayerSay","MultiChat",GlobalChat.Chat)



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
			cout("Sent buffer OK.")
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