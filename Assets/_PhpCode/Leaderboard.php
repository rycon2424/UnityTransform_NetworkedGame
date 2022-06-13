<?php
   
   include "Connection.php";

   $query = "SELECT username, score, lastplayed FROM UsersLogin ORDER BY convert(`score`, UNSIGNED INTEGER) DESC LIMIT 10";
   $result = $mysqli->query($query);

    while($row = mysqli_fetch_assoc($result))
    {
        $posts[] = $row;
    }

     echo "<br> <br> <b>  Top Players of all time </b> <br>";
     echo "-";
     for ($x = 1; $x < 11; $x++) 
     {
               $myObj->name = $posts[$x - 1];
               $myJson = json_encode($myObj);
               echo "<br> " . $myJson;
     }
    echo "<br> -";

     $queryThisMonth = "SELECT username, score, lastplayed FROM UsersLogin WHERE lastplayed BETWEEN (CURRENT_DATE() - INTERVAL 1 MONTH) AND CURRENT_DATE() ORDER BY convert(`score`, UNSIGNED INTEGER)";
     $thisMonthResult = $mysqli->query($queryThisMonth);

     while($rowMonth = mysqli_fetch_assoc($thisMonthResult))
     {
              $postsMonth[] = $rowMonth;
     }

     echo "<br> <br> <br> <b> Top Players this month: Unique playercount: " . count($postsMonth) . "</b>" ;
     echo "<br> -";
     for ($x = 1; $x < 11; $x++)
     {
               $myObjMonth->name = $postsMonth[$x - 1];
               $myJsonMonth = json_encode($myObjMonth);
               echo "<br> " . $myJsonMonth;
     }
     echo "<br> -";

?>