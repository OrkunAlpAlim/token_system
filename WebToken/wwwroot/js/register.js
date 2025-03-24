document.getElementById("userForm").addEventListener("submit", async function(event) {
    event.preventDefault();  // Sayfanın yenilenmesini engelle
    let name = document.getElementById("name").value;
    let surname = document.getElementById("surname").value;
    let username = document.getElementById("usename").value;
    let password = document.getElementById("passw").value;
    let resultElement = document.getElementById("result");
    
    let data = {
        Name: name,
        Surname: surname,
        Username: username,
        Password: password
    }; 

    try {
        let response = await fetch('api/Auth/Register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            resultElement.innerHTML = "Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz...";
            resultElement.style.color = "green";
            
            
            setTimeout(() => {
                window.location.href = "/";
            }, 2000);
        } else {
            let errorMessage = await response.text();
            resultElement.innerHTML = "Hata: " + errorMessage;
            resultElement.style.color = "red";
        }
    } catch (error) {
        console.error("Hata:", error);
        resultElement.innerHTML = "Bir hata oluştu!";
        resultElement.style.color = "red";
    }
});