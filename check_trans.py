import subprocess
result = subprocess.run(['sqlcmd', '-S', '.\\SQLEXPRESS', '-d', 'QuanLyChiTieuDb', '-Q', 'SET QUOTED_IDENTIFIER ON; SELECT COUNT(*) AS Total2026 FROM Transactions WHERE TransactionDate >= \'2026-01-01\' AND TransactionDate <= \'2026-12-31\';'], capture_output=True, text=True, errors='replace')
print('STDOUT:', result.stdout)
print('STDERR:', result.stderr)