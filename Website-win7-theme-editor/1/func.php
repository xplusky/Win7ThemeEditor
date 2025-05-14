<?php
try{
    $c = new SaeCounter;
}
catch(Exception $ex){
    die($ex->getMessage());
}
switch($_GET["arg"]){
    case "getdowncount":
        echo $c->get('W7TEDownCount');
        break;
    case "incrdowncount":
        $c->incr('W7TEDownCount');
        break;
        
}
?>