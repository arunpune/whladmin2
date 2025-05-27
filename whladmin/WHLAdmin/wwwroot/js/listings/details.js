$(function () {

  $('.whl-action-view-preview').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    const previewUrl = listingDetailsPreviewUrl + '?listingId=' + listingId;
    window.open(previewUrl, '_whldetailspreview', 'location=no,toolbar=no,menubar=no,scrollbars=yes,resizable=yes');
  });

  $('.whl-action-view-paperform').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    const previewUrl = listingViewPaperFormUrl + '?listingId=' + listingId;
    window.open(previewUrl, '_whlpreview', 'location=no,toolbar=no,menubar=no,scrollbars=yes,resizable=yes');
  });

  $('.whl-action-view-affordabilityanalysis').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(affordabilityAnalysisUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#affordabilityAnalysisViewerModal').find('div.modal-body').html(response);
        $('#affordabilityAnalysisViewerModal').modal('show');
      });
  });

});