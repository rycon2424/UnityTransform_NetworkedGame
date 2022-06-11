<?php

   include "Connection.php";

   $playerusername = $_POST["username"];
   $newscore = $_POST["score"];

   $oldscorequery = "SELECT score FROM UsersLogin WHERE username='$playerusername'";

   $result = $mysqli -> query($oldscorequery);
   $row = $result -> fetch_row();
   $result -> free_result();

    $highscoreQuery = "UPDATE UsersLogin SET score= '$newscore' WHERE username= '$playerusername' AND '(int)$newscore' > '(int)$row[0]'";
    $updateHighscore = $mysqli -> query($highscoreQuery);

    $result -> free_result();
    echo " 0";
?>