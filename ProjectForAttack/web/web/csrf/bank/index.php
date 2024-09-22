<?php

// error_reporting(E_ALL);
// ini_set('display_errors', 1);

try {

	$user = NULL;

	$db_host = 'postgres';
	$db_name = 'bank';
	$db_user = 'bank';
	$db_password = 'password';

	$action = isset($_POST['action']) ? $_POST['action'] : NULL;

	$dsn = "pgsql:host=$db_host;port=5432;dbname=$db_name;";
	$db = new PDO($dsn, $db_user, $db_password, [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]);

	if ($action === 'exit') {
		setcookie('auth', '', -1, "/csrf/bank", "", false, true);
		$usr = NULL;
	} else if ($action === 'enter') {
		$usr = $_POST['usr'];
		$pwd = '\x' . hash('sha256', $_POST['pwd']);
		$stmt = $db->prepare('SELECT * FROM "users" WHERE usr=? AND pwd=?');
		$stmt->execute([$usr, $pwd]);
		// usr, pwd, parrots
		$user = $stmt->fetchAll(PDO::FETCH_ASSOC);

		if (count($user) === 0) {
			$user = NULL;
			$error = "Введён неправильный логин " . $db->quote($usr) . " или пароль " . $db->quote($pwd);
		} else {
			$user = $user[0];
			setcookie('auth', base64_encode($user['usr']), -1, "/", "", false, true);
		}
	} else {
		$usr = base64_decode($_COOKIE['auth']);
		$stmt = $db->prepare('SELECT * FROM "users" WHERE usr=?');
		$stmt->execute([$usr]);
		$user = $stmt->fetchAll(PDO::FETCH_ASSOC);
		$user = count($user) === 0 ? NULL : $user[0];
		if ($user === NULL) {
			$error = htmlentities('Вы точно вошли в аккаунт? Авторизуйтесь заново.');
		} else if ($action === 'send') {
			$ammount = $_POST['ammount'];
			$to = $_POST['user'];
			$stmt = $db->prepare('SELECT * FROM "users" WHERE "usr"=?');
			$stmt->execute([$to]);
			$to_user = $stmt->fetchAll(PDO::FETCH_ASSOC);
			if (count($to_user) !== 0) {
				$db->prepare('UPDATE "users" SET parrots = parrots - ? WHERE "usr" = ?')->execute([$ammount, $usr]);
				$db->prepare('UPDATE "users" SET parrots = parrots + ? WHERE "usr" = ?')->execute([$ammount, $to]);
				$user['parrots'] = intval($user['parrots']) - intval($ammount);
				$message = htmlentities("Успешно отправлено $ammount попугаев пользователю $to");
			} else {
				$error = htmlentities("Пользователь $to не существует");
			}
		}
	}
} catch(Exception $e) {
	$error = $e->getMessage();
}

?>

<html>

<head>
	<meta charset="utf-8">
	<title>Попугай-банк</title>
	<link href="default.css" rel="stylesheet">
</head>

<body>
	
<?php if ($user !== NULL): ?>

	<h1>Попугай-монета</h1>

	<h2>Отсыпать немного чеканных попугаев другому человеку</h2>

	<form method="post" action="">
		<input type="submit" value="Выйти" class="exit">
		<input type="hidden" name="action" value="exit">
	</form>

	<form action="" method="post" class="default">
		<div class="ammount">У вас <?= $user['parrots'] ?> попугаев</div>
		<div>
			<label for="user">Кому</label>
			<input name="user" id="user" >		
		</div>
		<div>
			<label for="ammount">Сколько</label>
			<input name="ammount" type="number" min="0">
		</div>
		<input type="submit" value="Пересести попугаев">
		<input type="hidden" name="action" value="send">
	</form>

<?php else: ?>

	<h1>Чеканный попугай</h1>
	<h2>Авторизуйтесь для того, чтобы перевести другому человеку немного чеканных попугаев</h2>
	<form action="" method="post" class="default">
		<div>
			<label for="usr">Имя пользователя</label>
			<input type="text" id="usr" name="usr">	
		</div>
		<div>
			<label for="pwd">Пароль</label>
			<input type="password" name="pwd">
		</div>
		<div>
			<input type="submit" value="Войти">
		</div>
		<input type="hidden" name="action" value="enter">
	</form>

<?php endif; ?>

<?php if(isset($message)): ?>
	<div class="correct"><?= $message ?></div>
<?php endif; ?>

<?php if(isset($error)): ?>
	<div class="error"><?= $error ?></div>
<?php endif; ?>

</body>

</html>

