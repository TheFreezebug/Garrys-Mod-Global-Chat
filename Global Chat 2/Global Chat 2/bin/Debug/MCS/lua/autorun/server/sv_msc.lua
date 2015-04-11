require("glsock2");
if MSC then
	if MSC.Sock then
		MSC.Sock:Close()
	end
 
end 

MSC = {}
MSC.Server = "66.150.188.238"
MSC.Port = 9018
MSC.ServerID = "Fish"


// SEPERATOR CHAR IS 0x13

MSC.SepChar = string.char(0x13) // DO NOT TOUCH UNLESS YOU HAVE THE SERVER SOURCE AND YOU REALLY KNOW WHAT YOURE DOING.



util.AddNetworkString("MSCText")

local function cout(x) 
	Msg("[") MsgC(Color(255,0,255,255),"MSC") Msg("]: ") MsgN(x)
end 
GLSOCK_SUCCESS = GLSOCK_ERROR_SUCCESS
MSC.Sock = GLSock(GLSOCK_TYPE_TCP)
function MSC.Connect(sck,err) 
	if (err==GLSOCK_SUCCESS)  then
		// Send initial buffer to ID server.
		local buffer = GLSockBuffer()
		buffer:Write("id") // Write ID command 
		buffer:Write(MSC.SepChar) // Write byte seperator
		buffer:Write(MSC.ServerID) // Write string argument
		sck:Send(buffer,function()  //send buffer to server with callback of cout success.
			cout("Connection and Identification successful.")

			local y = MSC.ColorToString(Color(255,0,255,255))
			MSC.SendServerString({"msg",MSC.SepChar,y,MSC.SepChar," [SERVER] ",MSC.SepChar, " Connected."}) 
		
		end)
		sck:Read(1000,MSC.Read)//Read with callback of MSC.Read.
	else
		cout("Something went wrong, terminating scoket.")
		sck:Close()
		/*
		cout("Attempting reconnect in 15 seconds.")
		timer.Simple(15,function()
			MSC.Sock:Connect(MSC.Server,MSC.Port,MSC.Connect)

		end)
		*/

	end
end
function MSC.ColorToString(col) 
	local rstr = ""
	local X = {} // SORRY I HAVE TO DO THIS.
	X[1] = col.r // SORRY I HAVE TO DO THIS.
	X[2] = col.g // SORRY I HAVE TO DO THIS.
	X[3] = col.b // SORRY I HAVE TO DO THIS.
	X[4] = col.a // SORRY I HAVE TO DO THIS.
	// I have to maintain order of arguments so i can unpack() later :x.

	for k,v in pairs(X) do
		if k~=#col then 
			rstr = rstr .. v .. "|" 
		else
			rstr = rstr .. v
		end
	end
	return rstr
end
function MSC.ColorFromString(str) 
	//local ex = string.Explode("|",str)
	//PrintTable(ex)
	return Color(unpack(string.Explode("|",str)))
end
						--- TAKEN FROM SERVER SOURCE CODE
                   			//INCOMING NET MESSAGE FORMAT
                           //COMMAND|PLAYERCOLOR|PLRNAME|MESSAGE
                           //   0         1         2       3   // total values = 4 // NMSG


                           // OUTGOING NET MESSAGE FORMAT
                           //COMMAND|SERVERID|PLAYERCOLOR|PLRNAME|MESSAGE USE x VAR AS SEPERATOR! MAKE IDENTICAL SPLIT IN LUA CLIENT.
                           // 0         1         2        3         4-- NMsg  -- REMEMBER TO ADD ONE BECAUSE THIS IS LUA
                           //string servername = tcpClient.Client.RemoteEndPoint.To

function MSC.DataHandler(str)
	if not string.find(str,MSC.SepChar) then Error("DATA IS NOT VALID") return end 
	local ex = string.Explode(MSC.SepChar,str) 
	local command = ex[1]
	if command=="wng" then 
		cout("Server warning: " .. ex[2])
	end
	if command=="msg" then 
		local serverid = ex[2]
		local plcolor = MSC.ColorFromString(ex[3])

		local plname = ex[4]
		local msg = ex[5]
		local CTab = {Color(255,255,255,255),"[",serverid,"] ",plcolor,plname,Color(255,255,255,255) ,": " .. msg} // post to client
		
		
		for k,v in pairs(player.GetAll()) do 
			if v.GChatDisabled~=true then 
				net.Start("MSCText")
					net.WriteTable(CTab)
				net.Send(v)
			end
		end
		

	end
	--[[
	local serverid = ex[2]
	local plcolor = MSC.ColorFromString(ex[3])
	local plname = ex[4]
	local msg = ex[5]
	PrintTable(ex)
	--]]
end
function MSC.Chat(plr,t)
	t = string.gsub(t,MSC.SepChar,".")
	local tx = string.Explode(" ",t)

	if tx[1] then 
		if string.lower(tx[1])=="!global" then 
			if tx[2] then 
				if string.lower(tx[2])=="off" then 
					plr.GChatDisabled = true 
					plr:ChatPrint("Global chat has been disabled.")
				else 
					plr.GChatDisabled = false
					plr:ChatPrint("Global chat has been enabled.")
				end

			end 
	
		end
	end
	local plrname = plr:Nick()
	if plr.GChatDisabled==true then 
		plrname = "(local) " .. plrname
	end
	local plrcolor = team.GetColor(plr:Team())
	//local message = t
	MSC.SendServerString({"msg",MSC.SepChar,plrcolor,MSC.SepChar, plrname,MSC.SepChar,t})


end
hook.Add("PlayerSay","MultiChat",MSC.Chat)
function MSC.SendServerString(tab)
	if type(tab)~="table" then Error("Table expected got " .. type(tab)) end
		local buffer = GLSockBuffer()
		for k,v in pairs(tab) do
			if type(v)=="table" then 
				v = MSC.ColorToString(v)
			end
			buffer:Write(v)
		end
		MSC.Sock:Send(buffer,function()  end)
	

end 

function MSC.Read(sck, buf , err) // read the socket buffer
	if (err == GLSOCK_SUCCESS) then
		local c,data = buf:Read(buf:Size()) // read the complete buffer.
		if (c > 0) then 
			MSC.DataHandler(data) // Pass the (valid?) data on to the handler
		end
		sck:Read(1000,MSC.Read) // Keep reading.
	else
		cout("Something went wrong, terminating scoket.")
		sck:Close()
		cout("Attempting reconnect in 15 seconds.")
		timer.Simple(15,function()
			MSC.Sock:Connect(MSC.Server,MSC.Port,MSC.Connect)

		end)
	end 
end
MSC.Sock:Connect(MSC.Server,MSC.Port,MSC.Connect)