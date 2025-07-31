const { request, expect, test } = require('@playwright/test');

test('GET /posts/1 returns post with id 1', async () => {
  const context = await request.newContext();
  const response = await context.get('https://jsonplaceholder.typicode.com/posts/1');
  expect(response.status()).toBe(200);
  
  const json = await response.json();
  expect(json.id).toBe(1);
  expect(json.userId).toBe(1);
  expect(json).toHaveProperty('title');
});