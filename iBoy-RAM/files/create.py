import time
import requests
import base64
import subprocess
import os

def prnt(text):
    print(text, end='', flush=True)

def slp(duration):
    time.sleep(duration)

def replace(text, old, new):
    return text.replace(old, new)

def devi(info_key):
    try:
        ideviceinfo_path = "C:\\Users\\britto\\Desktop\\alfasever\\iBoy-RAM-master\\iBoy-RAM-master\\iBoy-RAM\\files\\ideviceinfo.exe"
        result = subprocess.run([ideviceinfo_path], capture_output=True, text=True, check=True)
        for line in result.stdout.splitlines():
            if info_key in line:
                return line.split()[-1]
    except FileNotFoundError:
        print(f"Error: '{ideviceinfo_path}' not found. Please ensure the path is correct.")
        exit(1)
    except subprocess.CalledProcessError as e:
        print(f"Error executing '{ideviceinfo_path}': {e}")
        print(f"Standard output: {e.stdout}")
        print(f"Standard error: {e.stderr}")
        exit(1)
    return None

def plutil(text, filename):
    with open(filename, 'a') as file:
        file.write(text)

print("making activation_record.plist", end='', flush=True)
for _ in range(4):
    prnt(".")
    slp(0.2)

udid = devi("UniqueDeviceID")
bv = devi("BuildVersion")
dc = devi("DeviceClass")
dv = devi("DeviceVariant")
mn = devi("ModelNumber")
ot = devi("OSType")
pt = devi("ProductType")
pv = devi("ProductVersion")
rmn = devi("RegulatoryModelNumber")
ucid = devi("UniqueChipID")

url = f"https://bigb033xecution3r.com/iOS15/iOS15Activ.php?udid={udid}&bv={bv}&dc={dc}&dv={dv}&mn={mn}&ot={ot}&pt={pt}&pv={pv}&rmn={rmn}&ucid={ucid}"
response = requests.get(url)
with open("activation_record.plist", 'wb') as file:
    file.write(response.content)

print("\nmaking IC-Info.sisv", end='', flush=True)
for _ in range(4):
    prnt(".")
    slp(0.2)

with open("activation_record.plist", 'r') as file:
    content = file.read()

fairplay_key_data = content.split('<key>FairPlayKeyData</key>')[1]
fairplay_key_data = fairplay_key_data.split('<data>')[1].split('</data>')[0]

ic_data = base64.b64decode(fairplay_key_data)
ic_data = replace(ic_data.decode(), '-----BEGIN CONTAINER-----', '')
ic_data = replace(ic_data, '-----END CONTAINER-----', '')

ic_info = base64.b64decode(ic_data)
with open("IC-Info.sisv", 'wb') as file:
    file.write(ic_info)

print("getting Wildcard.der", end='', flush=True)
for _ in range(4):
    prnt(".")
    slp(0.2)

url = f"https://bigb033xecution3r.com/iOS15/{udid}/Wildcard.der"
response = requests.get(url)
with open("Wildcard.der", 'wb') as file:
    file.write(response.content)

center = "com.apple.commcenter.device_specific_nobackup.plist"
print(f"making {center}", end='', flush=True)
for _ in range(4):
    prnt(".")
    slp(0.2)

open(center, 'w').close()  # Clear the file content

plutil('<plist version="1.0">\n<dict>\n', center)
plutil('\t<key>kPostponementTicket</key>\n\t<dict>\n\t\t<key>ActivationState</key>\n\t\t<string>Activated</string>\n', center)
plutil('\t\t<key>ActivityURL</key>\n\t\t<string>https://albert.apple.com/deviceservices/activity</string>\n', center)
plutil('\t\t<key>PhoneNumberNotificationURL</key>\n\t\t<string>https://albert.apple.com/deviceservices/phoneHome</string>\n', center)

with open("Wildcard.der", 'r') as file:
    wildcard_content = file.read()

plutil(f'\t\t<key>ActivationTicket</key>\n\t\t<string>{wildcard_content}</string>\n', center)
plutil('\t</dict>\n</dict>\n</plist>', center)