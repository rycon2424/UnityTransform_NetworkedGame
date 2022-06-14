<?php

include "UnityAutoLoginServer.php";

$playerusername = $_SESSION['username']; // Get user from session
$newscore = $_POST["score"];

if (empty($playerusername))
{
	echo "0 there is no player logged in the session right now, Log in here: https://studenthome.hku.nl/~bosko.ivkovic/KernMod_4/UserLoginWeb.php";
}

if (!filter_var($playerusername, FILTER_SANITIZE_STRING) || !filter_var($playerusername, FILTER_SANITIZE_STRING))
{
	echo "0 Error the score/username sended contains illigal characters";
}

$oldscorequery = "SELECT score FROM UsersLogin WHERE username='$playerusername'";

$result = $mysqli -> query($oldscorequery);
$row = $result -> fetch_row();
$result -> free_result();

$highscoreQuery = "UPDATE UsersLogin SET score= '$newscore' WHERE username= '$playerusername' AND '(int)$newscore' > '(int)$row[0]'";
$updateHighscore = $mysqli -> query($highscoreQuery) or die("0");

$updateTimeQuery = "UPDATE `UsersLogin` SET `lastplayed` = CURRENT_DATE() WHERE `UsersLogin`.`username` = '$playerusername';";
$updateTime = $mysqli -> query($updateTimeQuery) or die("0");

$result -> free_result();
echo "1";

?>