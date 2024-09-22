<?php 

function check_auth() {
  if (!isset($_COOKIE['auth'])) {
    return NULL;
  }

  $arr = explode('.', $_COOKIE['auth']);
  if (count($arr) != 2) {
    return NULL;
  }
  
  if (hash('md5', $arr[0] . 'yohoho') !== $arr[1]) {
    return NULL;
  }
  
  return $arr[0];
}

$db_host = 'postgres';
$db_name = 'game';
$db_user = 'racer';
$db_password = 'racer';

$template = 'login';

try {

  $dsn = "pgsql:host=$db_host;port=5432;dbname=$db_name;";
  $db = new PDO($dsn, $db_user, $db_password, [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]);

  $user_id = check_auth();
  if ($user_id === NULL) {
    $template = 'login';
  } else {
    $template = 'lk';
  }

	if ($_SERVER['REQUEST_METHOD'] === 'POST') {

    $action = isset($_POST['action']) ? $_POST['action'] : NULL;

    if ($action === 'login') {
        $user_name = $_POST['username'];
        $user_password = $_POST['password'];
    
        $query = 'SELECT * FROM "accounts" ' . 
                 'WHERE "login"=? ';

        $stmt = $db->prepare($query);
        $stmt->execute([$user_name]);
        $users = $stmt->fetchAll(PDO::FETCH_ASSOC);    
        $user = count($users) > 0 ? $users[0] : NULL;

        if ($user === NULL) {
          $template = 'user-error';
        } else {
          $password_hash = hash('md5', $user['sault'] . $user_password);
          if ($password_hash !== $user['password']) {
            $template = 'password-error';
          }  
          setcookie('auth', $user['id'] . '.' . hash('md5', $user['id'] . 'yohoho'), 0, '/race', '', false, true);
          $user_id = $user['id'];

          $template = 'lk';
        }
    } else {

      if ($user_id === NULL) {
        header('HTTP/1.1 403 Forbidden');
        exit(0);
      }
  
      if ($action === 'buy') {
        $query_1 = 'SELECT "money" FROM "accounts" where id=?';
        $stmt = $db->prepare($query_1);
        $stmt->execute([$user_id]);
        $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);
        $money = intval($rows[0]['money']);
  
        if ($money < 75000) {
          $template = 'no-enough-money';
        } else {
          $query_2 = 'INSERT INTO "payments"("account", "ammount", "text") VALUES (?, 75000, \'Заметный рост самого важного показателя! Все обзавидуются.\')';
          $stmt = $db->prepare($query_2);
          $stmt->execute([$user_id]);
          $query_3 = 'UPDATE "accounts" SET "money"="money"-75000 WHERE id=?';
          $stmt = $db->prepare($query_3);
          $stmt->execute([$user_id]);
        }
      }

      if ($action === 'return') {
        $id = $_POST['payment-id'];
        $query_1 = 'SELECT * FROM "payments" WHERE id=? AND account=? AND cancel=0';
        $stmt = $db->prepare($query_1);
        $stmt->execute([$id, $user_id]);
        $payments = $stmt->fetchAll(PDO::FETCH_ASSOC);
        if (count($payments) > 0) {
          sleep(1);
          $payment = $payments[0];
          $query_2 = 'UPDATE "accounts" SET "money"="money"+? WHERE id=?';
          $stmt = $db->prepare($query_2);
          $stmt->execute([intval($payment['ammount']), $user_id]);
          $query_3 = 'UPDATE "payments" SET "cancel"=1 WHERE id=?';
          $stmt = $db->prepare($query_3);
          $stmt->execute([$id]);
        } else {
          header('HTTP/1.1 400 Bad request');
          exit();
        }
        
      }

    }

	}

  $payments = [];
  $user = [];
  if ($template === 'lk') {
    $query = 'SELECT * FROM "payments" WHERE "account"=? AND "cancel"=0';
    $stmt = $db->prepare($query);
    $stmt->execute([$user_id]);
    $payments = $stmt->fetchAll(PDO::FETCH_ASSOC);
    
    $query = 'SELECT * FROM "accounts" WHERE "id"=?';
    $stmt = $db->prepare($query);
    $stmt->execute([$user_id]);
    $users = $stmt->fetchAll(PDO::FETCH_ASSOC);
    $user = $users[0];
  }

} catch (PDOException $e) {
  $error = $e->getMessage();
} finally {
	if (isset($pdo)) {
    $pdo = null;
	}
}

?>

<?php if (isset($error)): ?>
  <div class="error"><?= $error ?></div>
<?php endif; ?>

<!DOCTYPE html>
<html>
<head>
 <meta charset="utf-8">
 <title>Гонки</title>
 <link href="default.css" rel="stylesheet">
</head>
<body>

<?php if($template === 'user-error'): ?>
  <?php header('HTTP/1.1 403 Forbidden'); ?>
  <div>Пользователь не тот... Тот: Nagibator3000</div>
  <?php $template = 'login'; ?>
<?php endif; ?>

<?php if($template === 'password-error'): ?>
  <?php header('HTTP/1.1 403 Forbidden'); ?>
  <div>Пароль не тот... Тот: Nagibator3000</div>
  <?php $template = 'login'; ?>
<?php endif; ?>

<?php if($template === 'no-enough-money'): ?>
  <div>Нужно больше зо... Попугаев.</div>
  <div><a href="/race.php">Вернуться</a></div>
<?php endif; ?>

<?php if($template === 'lk'): ?>

  <section>
    <h1>Личный кабинет</h1>
    <div class="user"><?= $user['login'] ?> <a href="/race/exit.php">Выйти</a></div>
    <div class="money">Вы обладатель <span class="ammount"><?= $user['money'] ?></span> попугаев</div>
  </section>

  <form class="buy-booster"
        method="post"
        action="">
    <header>Покупка бустера</header>
    <input type="hidden" name="action" value="buy">
    <p>Хотите ощутить заметный рост самого важного показателя? Все обзавидуются!</p>
    <button>Жми меня!</button>
  </form>

  <?php foreach($payments as $item): ?>
    <form action="" method="post" class="return">
      <?= $item['text'] ?>
      <input name="payment-id" value="<?= $item['id'] ?>" type="hidden">
      <input name="action" value="return" type="hidden">
      <button>Вернуть</button>
    </form>
  <?php endforeach; ?>
<?php endif; ?>

<?php if($template === 'login'): ?>
  <form action="" method="post">
    <input name="action" value="login" type="hidden">
    <div>
      <label>Имя пользователя</label>
      <input name="username" type="text">
    </div>
    <div>
      <label>Пароль</label>
      <input name="password" type="password">
    </div>
    <div>
      <input type="submit" value="Войти">
    </div>
  </form>
<?php endif; ?>

</body>

<html>

