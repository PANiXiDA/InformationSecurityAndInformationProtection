<?php

// select MD5(format('%200000000s', 'yohoho')::bytea) from users;

$db_host = 'postgres';
$db_name = 'blind_auth';
$db_user = 'blind_auth_user';
$db_password = 'password';

try {

	if ($_SERVER['REQUEST_METHOD'] === 'POST') {
		$user_name = $_POST['username'];
		$user_password = $_POST['password'];
		$dsn = "pgsql:host=$db_host;port=5432;dbname=$db_name;";
		$db = new PDO($dsn, $db_user, $db_password, [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]);

		$password_hash = hash('sha256', $user_password);

		$query = "SELECT * FROM users \n" .
			 "WHERE password='" . '\x' . "$password_hash'::bytea \n" .
			 "  AND user = '$user_name'";

		$users = $db->query($query)->fetchAll(PDO::FETCH_ASSOC);
		$user = count($users) > 0 ? $users[0] : NULL;
	}

} catch (PDOException $e) {
	$error = $e->getMessage();
} finally {

}

?>

<!DOCTYPE html>
<html>
<head>
 <meta charset="utf-8">
 <title>Вход в мир внедрения кода SQL-injection вслепую без СМС и регистрации, жесть</title>

 <style>

html {
  background:#aaa;
}

.error {
  border:1px solid #f00;
  color:#f00;
  padding:10px 30px;
  margin:25px;
  background:#fee;
}

.success {
  border:1px solid #0f0;
  color:#262;
  margin:25px;
  padding:10px 30px;
  background:#efe;
}

pre {
  background:#fff;
  border:1px solid #000;
}

form {
  background:#fff;
  border-radius:15px;
  margin:25px auto;
  border:1px solid #282;
  max-width:400px;
  padding:25px;
}

label {
  color:#282;
  display:inline-block;
  width:180px;
}

div {
  margin:15px;
}

input {
  width:100px;
  border:1px solid #282;
  outline:#2f2;
  border-radius:8px;
  display:inline-block;
}

input[type=submit] {
  cursor:pointer;
  background:#282;
  color:#fff;
  border:1px solid #282;
  display:block;
  width:150px;
  margin:25px auto;
  padding:5px;
  text-align:center;
}

input[type=submit]:hover {
  color:#282;
  background:#fff;
}

 </style>

</head>
<body>

<?php if(isset($error)): ?>
<section class="error">
  <?= $error ?>
</section>
<?php endif; ?>

<?php if(isset($user) && $user !== NULL): ?>
<section class="success">
  Вы - какой-то пользователь. Поздравляю. Живите теперь с этим.
</section>
<?php endif; ?>

<?php if(isset($query)): ?>
<pre><?= $query ?></pre>
<?php endif; ?>

<form action="" method="post">
 <div>
  <label>Имя пользователя</label>
  <input type="text" id="username" name="username">
 </div>
 <div>
  <label>Пароль</label>
  <input type="text" id="password" name="password">
 </div>
 <input type="submit" value="Войти">
 <?php if(isset($user) && $user === NULL): ?>
 <section class="error">Ошибка аутентификации</section>
 <?php endif; ?>
</form>

</body>
