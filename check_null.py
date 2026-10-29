import subprocess
result = subprocess.run(['sqlcmd', '-S', '.\\SQLEXPRESS', '-d', 'QuanLyChiTieuDb', '-Q', "SET QUOTED_IDENTIFIER ON; SELECT COUNT(*) AS NullDate FROM Transactions WHERE TransactionDate IS NULL;"], capture_output=True, text=True, errors='replace')
print('STDOUT:', result.stdout)
print('STDERR:', result.stderr)