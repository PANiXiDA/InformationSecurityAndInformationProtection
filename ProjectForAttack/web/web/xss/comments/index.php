<?php 

$db_host = 'postgres';
$db_name = 'forum';
$db_user = 'comment_user';
$db_password = 'password';

try {
	$dsn = "pgsql:host=$db_host;port=5432;dbname=$db_name;";
	$db = new PDO($dsn, $db_user, $db_password, [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]);
	$show_auth_message = false;

	if ($_SERVER['REQUEST_METHOD'] === 'POST') {
		if (isset($_POST['clear'])) {
			// Очистить комментарий
			$query = 'DELETE FROM "comments"';
			$db->prepare($query)->execute();
		} else if (isset($_POST['comment'])) {		
			// Добавить комментарий
			$comment = $_POST['comment'];
			if ($comment) {
				$query = 'INSERT INTO "comments"("text") VALUES (?)';
				$db->prepare($query)->execute([ $comment ]);
			}
		} else if (isset($_POST['usr'])) {
			$show_auth_message = true;
		}
	}
	
	$query = "SELECT * FROM comments ORDER BY id DESC LIMIT 10";
	$comments = $db->query($query)->fetchAll(PDO::FETCH_ASSOC);
	
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
 <link href="default.css" rel="stylesheet" type="text/css" />
</head>
<body>

<header>
	<form action="" method="post">
		<?php if($show_auth_message): ?>
			<p>Вы успешно вошли в меня! Поздравляю! Страница, с которой отправлен запрос: <?= $_SERVER['HTTP_REFERER'] ?></p>
		<?php else: ?>
			<p>Войди в меня, о пользователь!</p>
		<?php endif; ?>
		<input type="text" placeholder="Логин" name="usr" >
		<input type="password" placeholder="Пароль" name="pwd" >
		<input type="submit" value="Войти" >
	</form>
</header>

<section>

	<h1 style="text-align:center">Комментарии восторssенных пользователей</h1>

	<form action="" method="post" >
		<textarea name="comment" placeholder="Оставить восторженный комментарий"></textarea>
		<input type="submit" value="Опубликовать">
	</form>
	<form action="" method="post" >
		<input type="submit" value="Очистить комментарии" name="clear">
	</form>
	<ul>
	  <?php foreach($comments as $comment): ?>
		<li><?= $comment['text'] ?></li>
	  <?php endforeach; ?>
	</ul>
	
</section>

</body>

</html>
