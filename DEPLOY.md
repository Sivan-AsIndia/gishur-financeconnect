# Running Gishur Finance Connect

You do **not** need the source code. You only need Docker and the file
`docker-compose.yml` that was sent to you.

---

## One-time setup

1. **Install Docker Desktop** — https://www.docker.com/products/docker-desktop
   Install it, then **start it**. Wait until the whale icon in your system tray
   stops animating and says "Docker Desktop is running".

2. **Make a folder** for the app, for example:
   - Windows: `C:\GishurFinance`
   - Mac: `~/GishurFinance`

3. **Save `docker-compose.yml` into that folder.** The file must keep exactly
   that name — not `docker-compose.yml.txt`.

---

## Starting the app

Open a terminal **in that folder**:

- **Windows** — open the folder in File Explorer, click the address bar, type
  `powershell` and press Enter.
- **Mac** — right-click the folder → Services → New Terminal at Folder.

Then run these two commands, one at a time:

```bash
docker compose pull
```
Downloads the app. Takes a few minutes the first time. Only downloads what
changed on later runs.

```bash
docker compose up -d
```
Starts the app. It waits for the backend to be ready before starting the
website, so this can sit for ~30 seconds before finishing.

Then open your browser to:

> **https://localhost:5002**

Your browser will show a **privacy / certificate warning**. This is expected —
the app uses a self-signed security certificate. Click **Advanced** →
**Continue to localhost**.

*(If that warning is a nuisance, http://localhost:5001 works with no warning.)*

You do **not** need a Docker Hub login. The images are public.

---

## Everyday commands

| What you want | Command |
|---|---|
| Start the app | `docker compose up -d` |
| Stop the app | `docker compose down` |
| Check if it's running | `docker compose ps` |
| Restart after a problem | `docker compose restart` |
| Update to a new version | `docker compose pull` then `docker compose up -d` |
| See error messages | `docker compose logs --tail 50` |

The app restarts automatically when you reboot your computer, as long as
Docker Desktop is set to start on login.

---

## Updating to a newer version

When you're told a new version is available, replace `docker-compose.yml` with
the new file you were sent, then run:

```bash
docker compose pull
docker compose up -d
```

Your data is kept. It lives in a Docker storage volume, not in the file.

---

## If something goes wrong

**"docker: command not found" / "The term 'docker' is not recognized"**
Docker Desktop is not installed, or not running. Start it and wait for the
whale icon to settle, then try again.

**"port is already allocated"**
Something else on your machine is using port 5001 or 5002. Stop that program,
or ask for a compose file with different ports.

**The page won't load**
Run `docker compose ps`. Both rows should say `Up`. If one says `Restarting` or
`Exited`, run `docker compose logs --tail 50` and send that output to whoever
gave you this file.

**You want to start completely fresh**
> ⚠️ This permanently deletes all accounts and data in the app.
```bash
docker compose down -v
docker compose up -d
```

---

## What is actually running

Two containers:

- `GishurFinanceConnect-api` — the backend. Not reachable from your browser
  directly; that's intentional.
- `GishurFinanceConnect` — the website you open, which also forwards requests
  to the backend.

Data is stored in a Docker volume called `gishur_finance-db-data`. Normal
`docker compose down` keeps it. Only `down -v` erases it.
