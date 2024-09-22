<?php

header('HTTP/1.1 302 Found');
header('Location: /race/');
setcookie('auth', '', -1, '/race', '', false, true);

?>