@{
     Layout = null;
} 
<!DOCTYPE html> 
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>Demo</title>
</head>
<body>
    <script src="/Scripts/jquery-3.5.1.js"></script>
    <script type="text/javascript">
        $(function () {
            $(".lowin-btn").click(function () {
                var user_name = $(":text").val();
                var password = $(":password").val();
                $.ajax({
                    url: "@Url.Action("login","Login")",
                    data: { username: user_name, password: password },
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "JSON",
                    success: function (data) {
                        alert(data);
                    }
                })
            })
        });
    </script>
</body>
</html>
