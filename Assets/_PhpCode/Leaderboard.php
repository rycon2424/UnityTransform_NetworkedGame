<?php

include "UnityAutoLoginServer.php";

$query = "SELECT username, score, lastplayed FROM UsersLogin ORDER BY convert(`score`, UNSIGNED INTEGER) DESC LIMIT 10";
$result = $mysqli->query($query);

while($row = mysqli_fetch_assoc($result))
{
	$posts[] = $row;
}

echo "<br> <br> <b> Top Players of all time </b> <br>";
echo "-";

foreach ($posts as $row)
{	
	foreach ($row as $element)
	{
		$i++;
		switch ($i)
		{
		case 1:
			$myObjT->Playername = $element;
		break;
		case 2:
			$myObjT->Score = $element;
	break;
		case 3:
			$myObjT->LastPlayed = $element;
			$myJsonT = json_encode($myObjT);
			echo "<br>" . $myJsonT;
			$i = 0;
		break;
		}
	}
}

echo "<br> _";

$queryThisMonth = "SELECT username, score, lastplayed FROM UsersLogin WHERE lastplayed BETWEEN (CURRENT_DATE() - INTERVAL 1 MONTH) AND CURRENT_DATE() ORDER BY convert(`score`, UNSIGNED INTEGER) DESC";
$thisMonthResult = $mysqli->query($queryThisMonth);

while($rowMonth = mysqli_fetch_assoc($thisMonthResult))
{
	$postsMonth[] = $rowMonth;
}

echo "<br> <br> <br> <b> Top Players this month: Unique playercount this month: " . count($postsMonth) . "</b>" ;
echo "<br> -";

$limit = 10;
foreach ($postsMonth as $rowMonth)
{
	$limit--;
	foreach ($rowMonth as $element)
	{
		$i++;
		switch ($i)
		{
		case 1:
			$myObjT->Playername = $element;
		break;
		case 2:
			$myObjT->Score = $element;
		break;
		case 3:
			$myObjT->LastPlayed = $element;
			$myJsonT = json_encode($myObjT);
			echo "<br>" . $myJsonT;
			$i = 0;
		break;
		}
	}
	if ($limit < 1)
	{
		break;
	}
}
echo "<br> _";

?>