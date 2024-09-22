<?php 

$db_host = 'postgres';
$db_name = 'auth';
$db_user = 'auth_user';
$db_password = 'password';

try {

	if ($_SERVER['REQUEST_METHOD'] === 'POST') {
		$user_name = $_POST['username'];
		$user_password = $_POST['password'];
		$dsn = "pgsql:host=$db_host;port=5432;dbname=$db_name;";
		$db = new PDO($dsn, $db_user, $db_password, [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]);

		$password_hash = hash('sha256', $user_password);

		$query = "SELECT * FROM \"users\" " . 
			 "WHERE \"user\"='$user_name' " . 
			 "AND \"password\"='\x$password_hash'";
		
		$users = $db->query($query)->fetchAll(PDO::FETCH_ASSOC);
		
		$user = count($users) > 0 ? $users[0] : NULL;
	}

} catch (PDOException $e) {
	$error = $e->getMessage();
} finally {
	if (isset($pdo)) {
		$pdo = null;
	}
}

?>

<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8">
  <title>Вход в мир внедрения кода SQL-injection</title>
  <link href="default.css" rel="stylesheet">
</head>
<body>

  <?php if(isset($error)): ?>
    <section class="error">
      <?= $error ?>
    </section>
  <?php endif; ?>

  <?php if(isset($user) && $user !== NULL): ?>
    <section class="success">
      Вы вошли от пользователя <?= $user['user'] ?>
    </section>
  <?php endif; ?>

  <?php if(isset($query)): ?>
    <pre><?= $query ?></pre>
    <pre><?= json_encode($users, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE); ?></pre>
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
