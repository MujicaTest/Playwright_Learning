const { request, test, expect } = require('@playwright/test');

test('POST /posts creates a new post', async () => {
  const context = await request.newContext();
  const payload = {
    title: 'QA Automation',
    body: 'Demo from Playwright',
    userId: 42
  };

  const response = await context.post('https://jsonplaceholder.typicode.com/posts', {
    data: payload
  });

  expect(response.status()).toBe(201);
  const json = await response.json();
  expect(json.title).toBe('QA Automation');
  expect(json.userId).toBe(42);
  expect(json).toHaveProperty('id');
});