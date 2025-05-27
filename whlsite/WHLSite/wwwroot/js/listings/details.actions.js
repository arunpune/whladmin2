$(function () {

  $('.whl-action-printlistingdetails').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    window.open(printListingDetailsUrl, '_whllistingdetails', 'location=no,toolbar=no,menubar=no,scrollbars=yes,resizable=yes');
  });

  $('.whl-action-printapplicationform').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    window.open(printApplicationFormUrl, '_whlappform', 'location=no,toolbar=no,menubar=no,scrollbars=yes,resizable=yes');
  });

  $('.whl-action-viewdocument').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const di = $(this).data('documentid');
    const a = document.createElement('a');
    a.download = '';
    a.href = viewDocumentUrl + '?documentid=' + di;
    a.click();
  });

  $('.whl-action-view-affordabilityanalysis').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(this).data('listingid');
    $.get(affordabilityAnalysistUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#affordabilityAnalysisViewerModal').find('div.modal-body').html(response);
        $('#affordabilityAnalysisViewerModal').modal('show');
      });
  });

});