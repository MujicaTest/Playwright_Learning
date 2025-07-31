import pytest
from playwright.sync_api import APIRequestContext, expect, sync_playwright

def test_get_post():
    with sync_playwright() as p:
        request_context: APIRequestContext = p.request.new_context()
        response = request_context.get("https://jsonplaceholder.typicode.com/posts/1")
        assert response.status == 200
        json = response.json()
        assert json["id"] == 1
        assert json["userId"] == 1
        assert "title" in json