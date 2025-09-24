# Web Directory
This directory contains the web interface for the Cura medication dispenser system.

## Files
- `index.html` - Main landing page

## Deployment
The web files are automatically deployed to `vm12.htl-leonding.ac.at` via GitHub Actions when changes are pushed to the main branch.

## Local Development
To test locally, simply open `index.html` in your browser or use a local web server:

```bash
# Using Python
python -m http.server 8000

# Using Node.js
npx serve .
```

Then visit: http://localhost:8000