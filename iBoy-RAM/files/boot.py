import subprocess
import time

def execute_command(command):
    try:
        print(f"Executando comando: {command}")
        subprocess.run(command, shell=True, check=True)
        print("Comando executado com sucesso!")
    except subprocess.CalledProcessError as e:
        print(f"Erro ao executar o comando: {command}")
        print(e)

commands = [
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -f C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\HXQ6XBKX\iboot.img4",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -f C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\HXQ6XBKX\iboot.img4",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -c go",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -f C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\HXQ6XBKX\devicetree.img4",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -c devicetree",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -f C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\HXQ6XBKX\ramdisk.img4",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -c ramdisk",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -f C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\HXQ6XBKX\rdtrust.img4",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -c firmware",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -f C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\HXQ6XBKX\kernel.img4",
    r"C:\Users\britto\Desktop\alfasever\iBoy-RAM-master\iBoy-RAM-master\iBoy-RAM\files\irecovery.exe -c bootx"
]

# Executa os comandos sequencialmente
for command in commands:
    execute_command(command)
    if "-c go" in command:
        print("Esperando 10 segundos...")
        time.sleep(10)  # Espera 10 segundos após o comando "go"
