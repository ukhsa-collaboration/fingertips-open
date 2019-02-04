function goToReportsPage() {

    setPageMode(PAGE_MODES.REPORTS);

    callAngularEvent('ReportsSelected');
}

pages.add(PAGE_MODES.REPORTS,
    {
        id: "reports",
        title: "Reports",
        goto: goToReportsPage,
        gotoName: 'goToReportsPage',
        needsContainer: false,
        jqIds: ['reports-container', 'areaMenuBox', 'parentTypeBox', 'areaTypeBox', 'region-menu-box']
    });
