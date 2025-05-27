$(function () {

  // Submit
  $('.whl-action-submit-listing').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#listingSubmitModal').modal('show');
    $('.whl-action-cancel-submit-listing').show();
    $('.whl-action-confirm-submit-listing').show();
    $('.whl-label-confirm-submit-listing-inprogress').hide();
  });
  $('.whl-action-confirm-submit-listing').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var listingId = $(e.currentTarget).data('listingid');
    $('.whl-action-cancel-submit-listing').hide();
    $('.whl-action-confirm-submit-listing').hide();
    $('.whl-label-confirm-submit-listing-inprogress').show();
    $.post(listingSubmitUrl)
      .done(function (response) {
        $('#listingSubmitModal').modal('hide');
        fnShowToast('SUCCESS', 'Listing #' + listingId + ' submitted for review, refreshing page.');
      })
      .fail(function (error) {
        $('#listingSubmitModal').modal('hide');
        var message = error.responseJSON && error.responseJSON.message && error.responseJSON.message.length > 0 ? error.responseJSON.message : 'Failed to submit Listing #' + listingId + ' due to one or more errors';
        if (error.responseJSON && error.responseJSON.details && error.responseJSON.details.length > 0) {
          message += '<br />' + error.responseJSON.details;
        }
        fnShowToastForPage('FAILURE', message);
      });
  });

  // Publish
  $('.whl-action-publish-listing').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#listingPublishModal').modal('show');
    $('.whl-action-cancel-publish-listing').show();
    $('.whl-action-confirm-publish-listing').show();
    $('.whl-label-confirm-publish-listing-inprogress').hide();
  });
  $('.whl-action-confirm-publish-listing').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var listingId = $(e.currentTarget).data('listingid');
    $('.whl-action-cancel-publish-listing').hide();
    $('.whl-action-confirm-publish-listing').hide();
    $('.whl-label-confirm-publish-listing-inprogress').show();
    $.post(listingPublishUrl)
      .done(function (response) {
        $('#listingPublishModal').modal('hide');
        fnShowToast('SUCCESS', 'Listing #' + listingId + ' published, refreshing page.');
      })
      .fail(function (error) {
        $('#listingPublishModal').modal('hide');
        var message = error.responseJSON && error.responseJSON.message && error.responseJSON.message.length > 0 ? error.responseJSON.message : 'Failed to publish Listing #' + listingId + ' due to one or more errors';
        if (error.responseJSON && error.responseJSON.details && error.responseJSON.details.length > 0) {
          message += '<br />' + error.responseJSON.details;
        }
        fnShowToastForPage('FAILURE', message);
      });
  });

  // Revise
  $('.whl-action-revise-listing').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#listingReviseModal').modal('show');
    $('.whl-formfield-revise-listing-reason').removeClass('is-valid');
    $('.whl-formfield-revise-listing-reason').removeClass('is-invalid');
    $('.whl-formfield-revise-listing-reason').removeAttr('readonly');
    $('.whl-formfield-revise-listing-reason').val('');
    $('.whl-action-cancel-revise-listing').show();
    $('.whl-action-confirm-revise-listing').show();
    $('.whl-label-confirm-revise-listing-inprogress').hide();
    fnToggleConfirmRevise();
  });
  $('.whl-formfield-revise-listing-reason').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleConfirmRevise();
  });
  $('.whl-action-confirm-revise-listing').on('click', function (e) {
    var listingId = $(e.currentTarget).data('listingid');
    var r = $('.whl-formfield-revise-listing-reason').val();
    var allowSubmission = (r !== undefined && r !== null && r.trim().length > 0);
    if (allowSubmission) {
      $('.whl-formfield-revise-listing-reason').attr('readonly', 'readonly');
      $('.whl-action-cancel-revise-listing').hide();
      $('.whl-action-confirm-revise-listing').hide();
      $('.whl-label-confirm-revise-listing-inprogress').show();
      const data = {
        ListingId: listingId,
        Reason: r,
      };
      $.post(listingReviseUrl, data)
        .done(function (response) {
          $('#listingReviseModal').modal('hide');
          fnShowToast('SUCCESS', 'Listing #' + listingId + ' returned for revisions, refreshing page.');
        })
        .fail(function (error) {
          $('#listingReviseModal').modal('hide');
          var message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to return Listing #' + listingId + ' for revisions due to one or more errors';
          fnShowToastForPage('FAILURE', message);
        })
        .always(function () {
          $('.whl-formfield-revise-listing-reason').removeClass('is-valid');
          $('.whl-formfield-revise-listing-reason').removeClass('is-invalid');
          $('.whl-formfield-revise-listing-reason').removeAttr('readonly');
          $('.whl-formfield-revise-listing-reason').val('');
        });
    }
  });
  function fnToggleConfirmRevise() {
    var v = $('.whl-formfield-revise-listing-reason').val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $('.whl-action-confirm-revise-listing').removeAttr('disabled');
    } else {
      $('.whl-action-confirm-revise-listing').attr('disabled', 'disabled');
    }
  }

  // Unpublish
  $('.whl-action-unpublish-listing').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#listingUnpublishModal').modal('show');
    $('.whl-formfield-unpublish-listing-reason').removeClass('is-valid');
    $('.whl-formfield-unpublish-listing-reason').removeClass('is-invalid');
    $('.whl-formfield-unpublish-listing-reason').removeAttr('readonly');
    $('.whl-formfield-unpublish-listing-reason').val('');
    $('.whl-action-cancel-unpublish-listing').show();
    $('.whl-action-confirm-unpublish-listing').show();
    $('.whl-label-confirm-unpublish-listing-inprogress').hide();
    fnToggleConfirmUnpublish();
  });
  $('.whl-formfield-unpublish-listing-reason').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleConfirmUnpublish();
  });
  $('.whl-action-confirm-unpublish-listing').on('click', function (e) {
    var listingId = $(e.currentTarget).data('listingid');
    var r = $('.whl-formfield-unpublish-listing-reason').val();
    var allowSubmission = (r !== undefined && r !== null && r.trim().length > 0);
    if (allowSubmission) {
      $('.whl-formfield-unpublish-listing-reason').attr('readonly', 'readonly');
      $('.whl-action-cancel-unpublish-listing').hide();
      $('.whl-action-confirm-unpublish-listing').hide();
      $('.whl-label-confirm-unpublish-listing-inprogress').show();
      const data = {
        ListingId: listingId,
        Reason: r,
      };
      $.post(listingUnpublishUrl, data)
        .done(function (response) {
          $('#listingUnpublishModal').modal('hide');
          fnShowToast('SUCCESS', 'Listing #' + listingId + ' sent for revisions from published listings, refreshing page.');
        })
        .fail(function (error) {
          $('#listingUnpublishModal').modal('hide');
          var message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to return published Listing #' + listingId + ' for revisions due to one or more errors';
          fnShowToastForPage('FAILURE', message);
        })
        .always(function () {
          $('.whl-formfield-unpublish-listing-reason').removeClass('is-valid');
          $('.whl-formfield-unpublish-listing-reason').removeClass('is-invalid');
          $('.whl-formfield-unpublish-listing-reason').removeAttr('readonly');
          $('.whl-formfield-unpublish-listing-reason').val('');
        });
    }
  });
  function fnToggleConfirmUnpublish() {
    var v = $('.whl-formfield-unpublish-listing-reason').val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $('.whl-action-confirm-unpublish-listing').removeAttr('disabled');
    } else {
      $('.whl-action-confirm-unpublish-listing').attr('disabled', 'disabled');
    }
  }

});