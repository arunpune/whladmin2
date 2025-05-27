$(function () {

  $('.whl-action-add-ami').on('click', function (e) {
    e.preventDefault();
    $.get(amiConfigAddUrl)
      .done(function (response) {
        $('#whl-title-ami-action').html('Add AMI');
        $('#hidAmiAction').val('ADD');
        $('#amiEditorModal').find('div.modal-body').html(response);
        $('#amiEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-ami').on('click', function (e) {
    e.preventDefault();
    const amiId = $(e.currentTarget).data('amiid');
    $.get(amiConfigEditUrl + '?effectiveDate=' + amiId)
      .done(function (response) {
        $('#whl-title-ami-action').html('Edit AMI');
        $('#hidAmiAction').val('EDIT');
        $('#amiEditorModal').find('div.modal-body').html(response);
        $('#amiEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-ami').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidAmiAction').val();
    const amiTitle = $('#EffectiveDate').val();
    const url = a === 'EDIT' ? amiConfigEditUrl : amiConfigAddUrl;
    const data = {
      EffectiveDate: $('#EffectiveDate').val(),
      EffectiveYear: $('#EffectiveYear').val(),
      IncomeAmt: $('#IncomeAmt').val(),
      HhPctAmts: [
        { Size: 1, Pct: $('#Hh1').val() },
        { Size: 2, Pct: $('#Hh2').val() },
        { Size: 3, Pct: $('#Hh3').val() },
        { Size: 4, Pct: $('#Hh4').val() },
        { Size: 5, Pct: $('#Hh5').val() },
        { Size: 6, Pct: $('#Hh6').val() },
        { Size: 7, Pct: $('#Hh7').val() },
        { Size: 8, Pct: $('#Hh8').val() },
        { Size: 9, Pct: $('#Hh9').val() },
        { Size: 10, Pct: $('#Hh10').val() }
      ],
      Hh1: $('#Hh1').val(),
      Hh2: $('#Hh2').val(),
      Hh3: $('#Hh3').val(),
      Hh4: $('#Hh4').val(),
      Hh5: $('#Hh5').val(),
      Hh6: $('#Hh6').val(),
      Hh7: $('#Hh7').val(),
      Hh8: $('#Hh8').val(),
      Hh9: $('#Hh9').val(),
      Hh10: $('#Hh10').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#amiEditorModal').find('div.modal-body').html('');
        $('#amiEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'AMI ' + amiTitle + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save AMI information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-ami').on('click', function (e) {
    e.preventDefault();
    const amiId = $(e.currentTarget).data('amiid');
    if (confirm('Are you sure you want to delete AMI - ' + amiId + '?')) {
      $.post(amiConfigDeleteUrl + '?effectiveDate=' + amiId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'AMI ' + amiId + ' deleted, refreshing page.');
        });
    }
  });

});