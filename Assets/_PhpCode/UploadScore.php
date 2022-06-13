<?php

   include "Connection.php";

   $playerusername = $_POST["username"];
   $newscore = $_POST["score"];

   $playerusername = filter_var($playerusername, FILTER_SANITIZE_STRING);
   $newscore = filter_var($newscore, FILTER_SANITIZE_STRING);

   $oldscorequery = "SELECT score FROM UsersLogin WHERE username='$playerusername'";

   $result = $mysqli -> query($oldscorequery);
   $row = $result -> fetch_row();
   $result -> free_result();

    $highscoreQuery = "UPDATE UsersLogin SET score= '$newscore' WHERE username= '$playerusername' AND '(int)$newscore' > '(int)$row[0]'";
    $updateHighscore = $mysqli -> query($highscoreQuery);

    $updateTimeQuery = "UPDATE `UsersLogin` SET `lastplayed` = CURRENT_DATE() WHERE `UsersLogin`.`username` = '$playerusername';";
    $updateTime = $mysqli -> query($updateTimeQuery);

    $result -> free_result();
    echo " 0";
?>