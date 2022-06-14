<?php

include "UnityAutoLoginServer.php";

$username=$_POST["name"];
$password=$_POST["password"];

if (!isset($username) || !isset($password))
{
	echo"ERROR CODE 8: No username or password received";
	exit();
}

if (!filter_var($username, FILTER_SANITIZE_STRING) || !filter_var($password, FILTER_SANITIZE_URL))
{
	echo"ERROR CODE 9: Use of illigal characters detected";
	exit();
}

$namecheckquery= "SELECT username FROM UsersLogin WHERE username= '$username';";

//echo "Searched for a duplicate of " . $username . $namecheckquery;
$namecheck = mysqli_query($mysqli,$namecheckquery) or die(" ERROR CODE 2: namecheck failed DATABASE ERROR");

if (mysqli_num_rows($namecheck) > 0)
{
	echo" ERROR CODE 3: Username already exists";
	exit();
}

$salt="\$5\$rounds=5000\$"."BoskoEncryption".$username."\$";
$hash=crypt($password,$salt);
$insertuserquery="INSERT INTO UsersLogin (username, hash, salt) VALUES ('$username','$hash','$salt');";
mysqli_query($mysqli,$insertuserquery)or die(" ERROR CODE 4: insert player failed" . $mysqli -> error);

echo "1";
?>