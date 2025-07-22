from playwright.sync_api import sync_playwright

def test_homepage_title():
    with sync_playwright() as p:
        browser = p.chromium.launch()
        page = browser.new_page()
        page.goto("https://playwright.dev")
        assert "Playwright" in page.title()
        browser.close()