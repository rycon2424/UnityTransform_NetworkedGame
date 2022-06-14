<?php

include "UnityAutoLoginServer.php";

$username=$_POST["name"];
$password=$_POST["password"];

if (!isset($username) || !isset($password))
{
	echo "0 ERROR CODE 8: No username or password received";
	exit();
}

if (!filter_var($username, FILTER_SANITIZE_STRING) || !filter_var($password, FILTER_SANITIZE_URL))
{
	echo "0 ERROR CODE 9: Use of illigal characters detected";
	exit();
}

$namecheckquery= "SELECT username, salt, hash, score FROM UsersLogin WHERE username= '$username';";

$namecheck = mysqli_query($mysqli, $namecheckquery) or die(" ERROR CODE 2: namecheck failed DATABASE Error");
if (mysqli_num_rows($namecheck) != 1)
{
	echo "0  ERROR CODE 6: User does not exist or entered wrong password";
	exit();
}

//get login info from query
$existinginfo = mysqli_fetch_assoc($namecheck);
$salt = $existinginfo["salt"];
$hash = $existinginfo["hash"];

$loginhash = crypt($password, $salt);
if ($hash != $loginhash)
{
	echo "0  ERROR CODE 6: User does not exist or entered wrong password";
	exit();
}

$_SESSION['username'] = $username;

echo "1 <br> hello ";
echo $_SESSION['username'];
echo "<br> Highscore:" . $existinginfo["score"];

?>

