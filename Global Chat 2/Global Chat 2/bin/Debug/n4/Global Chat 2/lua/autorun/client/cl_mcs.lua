net.Receive("MSCText",function() 
	chat.AddText(unpack(net.ReadTable()))

end)




	timer.Create("Fuckyouglobalchat",120,0,function() LocalPlayer():ChatPrint([[Tired of global chat? Type "!global off" in chat to rid of it, or "!global on" to re-enable it.]]) end)

