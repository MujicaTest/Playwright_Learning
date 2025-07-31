from playwright.sync_api import sync_playwright

def test_create_post():
    with sync_playwright() as p:
        request_context = p.request.new_context()
        payload = {
            "title": "QA Automation",
            "body": "Demo from Playwright",
            "userId": 42
        }
        response = request_context.post("https://jsonplaceholder.typicode.com/posts", data=payload)
        assert response.status == 201
        json = response.json()
        assert json["title"] == "QA Automation"
        assert json["userId"] == 42
        assert "id" in json  # Simulated ID from server

def test_404():
    with sync_playwright() as p:
        request_context = p.request.new_context()
        response = request_context.get("https://jsonplaceholder.typicode.com/invalid-endpoint")
        assert response.status == 404