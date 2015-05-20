<h1>Windows Phone Nabbar Plugin</h1>
<p>Since Windows Phone 8.1, some devices will have show the windows key, back and search keys on screen as soft keys instead of physical buttons. You can <a href="https://msdn.microsoft.com/en-us/library/windows/apps/xaml/dn792127.aspx" taget="_blank">read more here</a>. This causes a problem in Cordova/Phone Gap applications and the soft keys will overlap with the web view.</p>

<p>This plugin aims to sort that by automatically resizing the web view as the softkeys are shown/hidden by user action. If you need to, you can also capture the height of the soft key/navbar with the following:</p>

<code>
	window.addEventListener("navbarchange", function(info)
	{
		alert("the nav bar is " + info.height + " high!");
	}, false);
    
</code>

<p>Whilst this plugin uses properties that are only available on Windows Phone 8.1, it is backwards compatible and will run on earlier version but will do nothing.</p>