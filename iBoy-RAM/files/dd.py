from cryptography.hazmat.primitives.asymmetric import padding
from cryptography.hazmat.primitives.hashes import SHA256
from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.backends import default_backend

# Caminho para a chave privada
private_key_path = r'C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\cert.pem'

# Carregar a chave privada
with open(private_key_path, 'rb') as key_file:
    private_key = serialization.load_pem_private_key(
        key_file.read(),
        password=None,  # Substitua por sua senha, se a chave estiver protegida
        backend=default_backend()
    )

# Caminho para o arquivo criptografado
encrypted_file_path = r'C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\iPhone9,3-d101ap-15.enc'

# Ler o arquivo criptografado
with open(encrypted_file_path, 'rb') as enc_file:
    encrypted_data = enc_file.read()

# Descriptografar os dados
decrypted_data = private_key.decrypt(
    encrypted_data,
    padding.OAEP(
        mgf=padding.MGF1(algorithm=SHA256()),
        algorithm=SHA256(),
        label=None
    )
)

# Caminho para salvar o arquivo descriptografado
decrypted_file_path = r'C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\iPhone9,3-d101ap-15'

# Escrever os dados descriptografados em um arquivo
with open(decrypted_file_path, 'wb') as dec_file:
    dec_file.write(decrypted_data)

print(f"Arquivo descriptografado salvo como {decrypted_file_path}")
