document.getElementById("userForm").addEventListener("submit", async function(event) {
    event.preventDefault();  // Sayfanın yenilenmesini engelle
    let username= document.getElementById("username").value
    let password=document.getElementById("password").value
    let TokenText= document.getElementById("result")
    
    let data = {
       Username: username,
       Password: password
    }; 

    let response= await fetch('api/Auth/Login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
    let responseJson = await response.json();
    TokenText.innerHTML = responseJson.token;
    console.log(responseJson);
    console.log(response.body.Token);
    console.log(username, password);
});
