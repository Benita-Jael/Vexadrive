$base = 'http://localhost:5066'

function Pretty($label, $obj) { Write-Host "---- $label ----" -ForegroundColor Cyan; $obj | ConvertTo-Json -Depth 5; Write-Host "" }

# 1) Register customer
$customer = @{ name = 'E2E Customer'; email = 'e2e.customer+1@example.com'; password = 'E2E@1234'; confirmPassword = 'E2E@1234' }
try { $r = Invoke-RestMethod -Uri "$base/api/auth/register" -Method Post -ContentType 'application/json' -Body ($customer | ConvertTo-Json) -ErrorAction Stop; Pretty 'register' $r } catch { if ($_.Exception.Response) { $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $text = $sr.ReadToEnd(); Write-Host "REGISTER ERROR: $text" -ForegroundColor Red } else { Write-Host "REGISTER ERROR: $($_.Exception.Message)" -ForegroundColor Red } }

# 2) Login customer
$login = @{ email = $customer.email; password = $customer.password }
$token = $null
try { $r = Invoke-RestMethod -Uri "$base/api/auth/login" -Method Post -ContentType 'application/json' -Body ($login | ConvertTo-Json) -ErrorAction Stop; Pretty 'login' $r; $token = $r.token } catch { if ($_.Exception.Response) { $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $text = $sr.ReadToEnd(); Write-Host "LOGIN ERROR: $text" -ForegroundColor Red } else { Write-Host "LOGIN ERROR: $($_.Exception.Message)" -ForegroundColor Red } }

if (-not $token) { Write-Host 'No token; aborting further tests' -ForegroundColor Yellow; exit 1 }

# 3) Create a vehicle (customer endpoint may be /api/vehicle/add)
$vehicle = @{ model='Toyota Prius'; numberPlate='E2E-1234'; type='Car'; color='Silver' }
try { $r = Invoke-RestMethod -Uri "$base/api/vehicle/add" -Method Post -ContentType 'application/json' -Body ($vehicle | ConvertTo-Json) -Headers @{ Authorization = "Bearer $token" } -ErrorAction Stop; Pretty 'create vehicle' $r } catch { if ($_.Exception.Response) { $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $text = $sr.ReadToEnd(); Write-Host "CREATE VEHICLE ERROR: $text" -ForegroundColor Red } else { Write-Host "CREATE VEHICLE ERROR: $($_.Exception.Message)" -ForegroundColor Red } }

# 4) Create service request (customer)
$request = @{ vehicleId = ($r.vehicleId | ? { $_ -ne $null }) ; problemDescription='Engine making noise'; serviceAddress='123 Test Lane'; serviceDate = (Get-Date).AddDays(2).ToString('s') }
# If vehicleId not present, try get vehicles
if (-not $request.vehicleId) {
    try { $v = Invoke-RestMethod -Uri "$base/api/vehicle/all" -Method Get -Headers @{ Authorization = "Bearer $token" } -ErrorAction Stop; Pretty 'vehicles' $v; $request.vehicleId = $v[0].vehicleId } catch { Write-Host 'Unable to fetch vehicles' -ForegroundColor Yellow }
}
try { $r2 = Invoke-RestMethod -Uri "$base/api/customer/requests" -Method Post -ContentType 'application/json' -Body ($request | ConvertTo-Json) -Headers @{ Authorization = "Bearer $token" } -ErrorAction Stop; Pretty 'create request' $r2 } catch { if ($_.Exception.Response) { $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $text = $sr.ReadToEnd(); Write-Host "CREATE REQUEST ERROR: $text" -ForegroundColor Red } else { Write-Host "CREATE REQUEST ERROR: $($_.Exception.Message)" -ForegroundColor Red } }

# 5) Admin login
$adminLogin = @{ email = 'admin@vexadrive.com'; password = 'Admin@123' }
$adminToken = $null
try { $r = Invoke-RestMethod -Uri "$base/api/auth/login" -Method Post -ContentType 'application/json' -Body ($adminLogin | ConvertTo-Json) -ErrorAction Stop; Pretty 'admin login' $r; $adminToken = $r.token } catch { if ($_.Exception.Response) { $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $text = $sr.ReadToEnd(); Write-Host "ADMIN LOGIN ERROR: $text" -ForegroundColor Red } else { Write-Host "ADMIN LOGIN ERROR: $($_.Exception.Message)" -ForegroundColor Red } }

if (-not $adminToken) { Write-Host 'Admin login failed; cannot proceed to admin tests' -ForegroundColor Yellow; exit 1 }

# 6) Get all requests (admin)
try { $all = Invoke-RestMethod -Uri "$base/api/admin/requests" -Method Get -Headers @{ Authorization = "Bearer $adminToken" } -ErrorAction Stop; Pretty 'admin requests' $all } catch { if ($_.Exception.Response) { $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $text = $sr.ReadToEnd(); Write-Host "ADMIN REQUESTS ERROR: $text" -ForegroundColor Red } else { Write-Host "ADMIN REQUESTS ERROR: $($_.Exception.Message)" -ForegroundColor Red } }

# 7) If there's at least one request, update its status and upload a bill
if ($all -and $all.Count -gt 0) {
    $reqId = $all[0].serviceRequestId
    # update status to ServiceInProgress
    try { $u = Invoke-RestMethod -Uri "$base/api/admin/requests/$reqId/status" -Method Put -ContentType 'application/json' -Body (ConvertTo-Json @{ status='ServiceInProgress' }) -Headers @{ Authorization = "Bearer $adminToken" } -ErrorAction Stop; Pretty 'update status' $u } catch { if ($_.Exception.Response) { $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $text = $sr.ReadToEnd(); Write-Host "UPDATE STATUS ERROR: $text" -ForegroundColor Red } else { Write-Host "UPDATE STATUS ERROR: $($_.Exception.Message)" -ForegroundColor Red } }

    # create dummy bill file
    $tmp = Join-Path $PSScriptRoot 'dummy-bill.pdf'
    "This is a test bill" | Out-File -FilePath $tmp -Encoding utf8

    # upload bill via curl to support multipart
    $curl = "curl -s -X POST '$base/api/admin/requests/$reqId/bill' -H 'Authorization: Bearer $adminToken' -F 'file=@$tmp;type=application/pdf' -w '%{http_code}'"
    Write-Host "Running: $curl"
    $status = iex $curl
    Write-Host "Upload result: $status"
}

# 8) Check notifications for customer
try { $notes = Invoke-RestMethod -Uri "$base/api/customer/notifications" -Method Get -Headers @{ Authorization = "Bearer $token" } -ErrorAction Stop; Pretty 'customer notifications' $notes } catch { if ($_.Exception.Response) { $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $text = $sr.ReadToEnd(); Write-Host "NOTIFICATIONS ERROR: $text" -ForegroundColor Red } else { Write-Host "NOTIFICATIONS ERROR: $($_.Exception.Message)" -ForegroundColor Red } }

Write-Host 'E2E script finished' -ForegroundColor Green
