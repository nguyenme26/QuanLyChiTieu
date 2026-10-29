import subprocess
result = subprocess.run(['sqlcmd', '-S', '.\\SQLEXPRESS', '-d', 'QuanLyChiTieuDb', '-Q', "SET QUOTED_IDENTIFIER ON; SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Transactions' AND COLUMN_NAME = 'TransactionDate';"], capture_output=True, text=True, errors='replace')
print('STDOUT:', result.stdout)
print('STDERR:', result.stderr)