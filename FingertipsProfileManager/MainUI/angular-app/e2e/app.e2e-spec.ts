import { FpmAppPage } from './app.po';

describe('fpm-app App', () => {
  let page: FpmAppPage;

  beforeEach(() => {
    page = new FpmAppPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
